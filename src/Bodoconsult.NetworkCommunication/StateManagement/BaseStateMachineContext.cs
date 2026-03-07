// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.StateManagement;

public abstract class BaseStateMachineContext : BaseOrderManagementDevice, IStateManagementDevice
{
    private readonly ConcurrentQueue<IJobStateMachineState> _concurrentQueue = new();

    private IStateMachineState _currentState;
    private readonly Lock _currentStateLockObj = new();

    private IJobStateMachineState _savedJobState;
    private readonly Lock _savedJobStateLockObj = new();

    ///// <summary>
    /////  The current device
    ///// </summary>
    //public IOrderManagementDevice Device { get; protected set; }

    protected BaseStateMachineContext(IDataMessagingConfig dataMessagingConfig,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IDeviceStateCheckManager deviceStateCheckManager, IStateMachineStateFactory stateMachineStateFactory,
        int previousBusinessStateId) : base(dataMessagingConfig, clientNotificationManager)
    {
        DeviceStateCheckManager = deviceStateCheckManager;
        StateMachineStateFactory = stateMachineStateFactory;
        PreviousBusinessStateId = previousBusinessStateId;
    }

    /// <summary>
    /// Current device state check manager instances
    /// </summary>
    public IDeviceStateCheckManager DeviceStateCheckManager { get; protected set; }

    /// <summary>
    /// Current implementation of <see cref="IStateMachineStateFactory"/>
    /// </summary>
    public IStateMachineStateFactory StateMachineStateFactory { get; protected set; }

    /// <summary>
    /// Current job states waiting to be processed
    /// </summary>
    public List<IJobStateMachineState> JobStates => _concurrentQueue.ToList();

    /// <summary>
    /// Current device hardware state
    /// </summary>
    public IDeviceState DeviceState { get; protected set; } = DefaultDeviceStates.DeviceStateOffline;

    /// <summary>
    /// Current business substate
    /// </summary>
    public IBusinessSubState BusinessSubState { get; protected set; } = DefaultBusinessSubStates.NotSet;

    /// <summary>
    /// Previous device hardware state
    /// </summary>
    public IDeviceState PreviousDeviceState { get; protected set; } = DefaultDeviceStates.DeviceStateOffline;

    /// <summary>
    /// Previous business state
    /// </summary>
    public int PreviousBusinessStateId { get; protected set; }

    /// <summary>
    /// Previous business substate
    /// </summary>
    public IBusinessSubState PreviousBusinessSubState { get; protected set; } = DefaultBusinessSubStates.NotSet;

    /// <summary>
    /// Initiate the context
    /// </summary>
    public virtual void InitiateContext()
    {
        // Do nothing
    }

    /// <summary>
    /// Register a job state to execute
    /// </summary>
    /// <param name="jobState">Job state to execute</param>
    public void RegisterJobState(IJobStateMachineState jobState)
    {
        _concurrentQueue.Enqueue(jobState);
    }

    /// <summary>
    /// The current state
    /// </summary>
    public virtual IStateMachineState CurrentState
    {
        get
        {
            lock (_currentStateLockObj)
            {
                return _currentState;
            }
        }
        protected set
        {
            lock (_currentStateLockObj)
            {
                _currentState = value;
            }
        }
    }

    /// <summary>
    /// The last job state not processed completely
    /// </summary>
    public virtual IJobStateMachineState SavedJobState
    {
        get
        {
            lock (_savedJobStateLockObj)
            {
                return _savedJobState;
            }
        }
        protected set
        {
            lock (_savedJobStateLockObj)
            {
                _savedJobState = value;
            }
        }
    }

    /// <summary>
    /// Request a new state
    /// </summary>
    /// <param name="newState">New request to load</param>
    public virtual void RequestState(IStateMachineState newState)
    {
        // Activate or deactivate the regular state check
        if (newState.IsTurningOffStateRequestsRequired)
        {
            DeviceStateCheckManager.DeactivateStateCheck();
        }
        else
        {
            DeviceStateCheckManager.ActivateStateCheck();
        }

        // Cancel the old state
        CurrentState?.CancelState();

        // Save previous business state ID
        PreviousBusinessStateId = CurrentState?.Id ?? 0;

        // Now set the new state and run it
        CurrentState = newState;

        // Now set the initial states
        CurrentState.SetInitalStates();

        // Initiate the state with orders etc.
        CurrentState.InitiateState();

        // Run the first order now
        CurrentState.RunNextOrder();
    }

    /// <summary>
    /// Request a new device state. Do NOT use this method directly. Call <see cref="IStateMachineState.RequestNewDeviceState"/> instead to let the current state check the requested new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    public void RequestNewDeviceState(IDeviceState newDeviceState)
    {
        DeviceState = newDeviceState;
    }

    /// <summary>
    /// Create a state machine state
    /// </summary>
    /// <param name="stateName">Name of the state to create</param>
    /// <returns>Fresh instance of the requested state</returns>
    public IStateMachineState CreateStateInstance(string stateName)
    {
        return StateMachineStateFactory.CreateInstance(stateName);
    }

    /// <summary>
    /// Set all states
    /// </summary>
    /// <param name="deviceState">Current device state</param>
    /// <param name="businessSubState">Current business state</param>
    public void SetStates(IDeviceState deviceState, IBusinessSubState businessSubState)
    {
        // Save the current state
        PreviousDeviceState = DeviceState;

        PreviousBusinessSubState = BusinessSubState;

        // Now set the new states
        DeviceState = deviceState;
        BusinessSubState = businessSubState;

        // Log the changes
        LogStates();
    }

    /// <summary>
    /// Method to implement logging or client notification on state changes
    /// </summary>
    public virtual void LogStates()
    {
        //LogInformation($"State changed from {PreviousDeviceState}/{PreviousBusinessState}/{PreviousBusinessSubState} to {DeviceState}/{BusinessState}/{BusinessSubState}");
    }

    /// <summary>
    /// The communication to the device was broken. Handle this event
    /// </summary>
    public void HandleComDevClose()
    {
        CurrentState.HandleComDevClose();
    }

    /// <summary>
    /// Restore the saved job state
    /// </summary>
    public void RestoreSavedState()
    {
        // Is there a saved job state to be restarted?
        if (SavedJobState == null)
        {
            return;
        }

        // Activate or deactivate the regular state check
        if (SavedJobState.IsTurningOffStateRequestsRequired)
        {
            DeviceStateCheckManager.DeactivateStateCheck();
        }
        else
        {
            DeviceStateCheckManager.ActivateStateCheck();
        }

        // Cancel the old state
        CurrentState?.CancelState();

        // Now set the new state but do not initate it as it was initated already
        CurrentState = SavedJobState;
        SavedJobState = null;
    }

    /// <summary>
    /// Save the state if it implements <see cref="IJobStateMachineState"/>
    /// </summary>
    /// <param name="stateMachineState">Current state</param>
    public void SaveJobState(IStateMachineState stateMachineState)
    {
        if (stateMachineState is not IJobStateMachineState js)
        {
            return;
        }

        SavedJobState = js;
    }

    /// <summary>
    /// Set the business sub state
    /// </summary>
    /// <param name="businessSubState">New busienss sub state to set</param>
    public void SetBusinessSubState(IBusinessSubState businessSubState)
    {
        // Save the current state
        PreviousBusinessSubState = BusinessSubState;

        // Now set the new state
        BusinessSubState = businessSubState;

        // Log the changes
        LogStates();
    }


}