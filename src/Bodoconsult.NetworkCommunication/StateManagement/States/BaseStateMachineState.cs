// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Base class for <see cref="IStateMachineState"/> implementations
/// </summary>
public abstract class BaseStateMachineState : IStateMachineState
{
    protected int CurrentOrderIndex;
    private bool _isErrorHandlingRunning;
    private readonly Lock _isErrorHandlingRunningLock = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    protected BaseStateMachineState(IStateManagementDevice currentContext, int id, string name)
    {
        CurrentContext = currentContext;
        CurrentOrderIndex = 0;
        Id = id;
        Name = name;
    }

    protected bool IsErrorHandlingRunning
    {
        get
        {
            lock (_isErrorHandlingRunningLock)
            {
                return _isErrorHandlingRunning;
            }
        }
        set
        {
            lock (_isErrorHandlingRunningLock)
            {
                _isErrorHandlingRunning = value;
            }
        }
    }

    /// <summary>
    /// State ID
    /// </summary>
    public int Id { get;  }

    /// <summary>
    /// Name of the state
    /// </summary>
    public string Name { get;  }

    /// <summary>
    /// The states allowed to follow the current state
    /// </summary>
    public List<string> AllowedNextStates { get; } = new();

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementDevice CurrentContext { get; }

    /// <summary>
    /// Initial device state. Default DeviceStateOffline
    /// </summary>
    public DeviceState InitialDeviceState { get; set; } = DefaultDeviceStates.DeviceStateOffline;

    /// <summary>
    /// Initial business substate. Default: NotSet
    /// </summary>
    public BusinessSubState InitialBusinessSubState { get; set; } = DefaultBusinessSubStates.NotSet;

    /// <summary>
    /// Is the cancellation of all running orders required before the state is applied
    /// </summary>
    public bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Is turning offstate request messages to the tower required
    /// </summary>
    public bool IsTurningOffStateRequestsRequired { get; set; }

    /// <summary>
    /// <see cref="IStateMachineState.CancellationTokenSource"/> instance to use for the current state or null if none is used
    /// </summary>
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    /// <summary>
    /// Delegate to cancel the state
    /// </summary>
    public CancelStateDelegate? CancelStateDelegate { get; set; }

    /// <summary>
    /// The next state to be requested when this state has to be left or null if the state does not change
    /// </summary>
    public IStateMachineState? NextState { get; set; }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    public virtual void SetInitalStates()
    {
        CurrentContext.SetStates(InitialDeviceState, InitialBusinessSubState);
    }

    /// <summary>
    /// Initiate this state
    /// </summary>
    public virtual void InitiateState()
    {
        // Do nothing
    }

    /// <summary>
    /// Cancel this state
    /// </summary>
    public virtual void CancelState()
    {
        // Do nothing
    }

    /// <summary>
    /// Request the next state set by the current state
    /// </summary>
    public void RequestNextState()
    {
        if (NextState == null)
        {
            return;
        }

        CurrentContext.RequestState(NextState);
    }

    /// <summary>
    /// The name of the state to be called if a ComDevClose event has happend. Default: DefaultStateNames.DeviceOfflineState
    /// </summary>
    public string StateToRequestOnComDevClose { get; set; } = DefaultStateNames.DeviceOfflineState;

    /// <summary>
    /// Delegate to handle a ComDevClose event
    /// </summary>
    public HandleComDevCloseDelegate? HandleComDevCloseDelegate { get; set; }


    /// <summary>
    /// The communication to the device was broken. Handles this event
    /// </summary>
    public virtual void HandleComDevClose()
    {
        // Save current job state if necessary
        CurrentContext.SaveJobState(this);

        // Reset internal state
        CurrentContext.ResetInternalState();

        // Now go to offline
        var state = CurrentContext.CreateStateInstance(StateToRequestOnComDevClose);
        NextState = state;

        // Fire the delegate to enable buisness logic reaction
        HandleComDevCloseDelegate?.Invoke(this);
    }

    /// <summary>
    /// The name of the state to be called if an error message was sent by the device. Default: DefaultStateNames.DeviceOfflineState
    /// </summary>
    public string StateToRequestOnError { get; set; } = DefaultStateNames.DeviceOfflineState;

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    public HandleErrorMessageDelegate? HandleErrorMessageDelegate { get; set; }

    /// <summary>
    /// Handle a received error message from the device. Default implementation calls <see cref="HandleErrorMessageDelegate "/> and then goes to state DefaultStateNames.DeviceOfflineState. Overwrite this method if other behaviour is required
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    public virtual void HandleErrorMessage(IInboundDataMessage receivedMessage)
    {
        if (IsErrorHandlingRunning)
        {
            return;
        }

        IsErrorHandlingRunning = true;

        try
        {
            if (HandleErrorMessageDelegate != null)
            {
                HandleErrorMessageDelegate.Invoke(this, receivedMessage);
            }

            // Now go to offline
            var state = CurrentContext.CreateStateInstance(StateToRequestOnComDevClose);
            NextState = state;

        }
        catch (Exception e)
        {
            CurrentContext.LogError(e.ToString());
            // Do nothing
        }
        finally
        {
            IsErrorHandlingRunning = false;
        }
    }

    /// <summary>
    /// Handle an async received message
    /// </summary>
    public HandleAsyncMessageDelegate? HandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle async sent message from device
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    public virtual MessageHandlingResult HandleAsyncMessage(IInboundDataMessage? message)
    {
        return HandleAsyncMessageDelegate == null ? 
            MessageHandlingResultHelper.Success() : 
            HandleAsyncMessageDelegate.Invoke(this, message);
    }

    /// <summary>
    /// Request a new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    public virtual void RequestNewDeviceState(IDeviceState newDeviceState)
    {
        CurrentContext.RequestNewDeviceState(newDeviceState);
    }

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    public virtual List<IOrder> PerpareRegularStateRequest()
    {
        var orders = new List<IOrder>();

        if (PrepareRegularStateRequestDelegate != null)
        {
            orders.AddRange(PrepareRegularStateRequestDelegate.Invoke());
        }

        return orders;
    }

    /// <summary>
    /// Delegate for preparing orders for the regular state reqeust
    /// </summary>
    public PrepareRegularStateRequestDelegate? PrepareRegularStateRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling device state check request answers in business logic
    /// </summary>
    public HandleRegularStateRequestAnswerDelegate? HandleRegularStateRequestAnswerDelegate { get; set; }

    /// <summary>
    /// Check a received state message from device and handle it
    /// </summary>
    /// <param name="message">Received state message from device</param>
    /// <param name="doNotNotifyClient">Do notify the clients</param>
    public MessageHandlingResult HandleRegularStateRequestAnswer(IInboundDataMessage message, bool doNotNotifyClient)
    {
        return HandleRegularStateRequestAnswerDelegate == null ? 
            MessageHandlingResultHelper.Success() : 
            HandleRegularStateRequestAnswerDelegate.Invoke(this, message, doNotNotifyClient);
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Id} {Name}";
    }
}