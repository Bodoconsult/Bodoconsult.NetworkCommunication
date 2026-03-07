// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// The context for a state machine implementation
/// </summary>
public interface IStateMachineContext
{
    /// <summary>
    /// The current state
    /// </summary>
    IStateMachineState CurrentState { get; }

    /// <summary>
    /// The last job state not processed completely
    /// </summary>
    IJobStateMachineState SavedJobState { get; }


    ///// <summary>
    /////  The current device
    ///// </summary>
    //IOrderManagementDevice Device { get; }

    /// <summary>
    /// Current device state check manager instances
    /// </summary>

    IDeviceStateCheckManager DeviceStateCheckManager { get; }

    /// <summary>
    /// Current implementation of <see cref="IStateMachineStateFactory"/>
    /// </summary>
    IStateMachineStateFactory StateMachineStateFactory { get; }


    /// <summary>
    /// Current job states waiting to be processed
    /// </summary>
    List<IJobStateMachineState> JobStates { get; }

    /// <summary>
    /// Current device hardware state
    /// </summary>
    IDeviceState DeviceState { get; }

    /// <summary>
    /// Current business substate
    /// </summary>
    IBusinessSubState BusinessSubState { get; }

    /// <summary>
    /// Previous device hardware state
    /// </summary>
    IDeviceState PreviousDeviceState { get; }

    /// <summary>
    /// Previous business state
    /// </summary>
    int PreviousBusinessStateId { get; }

    /// <summary>
    /// Previous business substate
    /// </summary>
    IBusinessSubState PreviousBusinessSubState { get; }

    /// <summary>
    /// Initiate the context
    /// </summary>
    void InitiateContext();

    /// <summary>
    /// Register a job state to execute
    /// </summary>
    /// <param name="jobState">Job state to execute</param>
    void RegisterJobState(IJobStateMachineState jobState);


    /// <summary>
    /// Request a new state
    /// </summary>
    /// <param name="newState">New request to load</param>
    void RequestState(IStateMachineState newState);

    /// <summary>
    /// Request a new device state. Do NOT use this method directly. Call <see cref="IStateMachineState.RequestNewDeviceState"/> instead to let the current state check the requested new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    void RequestNewDeviceState(IDeviceState newDeviceState);

    /// <summary>
    /// Create a state machine state
    /// </summary>
    /// <param name="stateName">Name of the state to create</param>
    /// <returns>Fresh instance of the requested state</returns>
    IStateMachineState CreateStateInstance(string stateName);

    /// <summary>
    /// Set all states
    /// </summary>
    /// <param name="deviceState">Current device state</param>
    /// <param name="businessSubState">Current business state</param>
    void SetStates(IDeviceState deviceState, IBusinessSubState businessSubState);

    /// <summary>
    /// Set the business substate
    /// </summary>
    /// <param name="businessSubState">New busienss sub state to set</param>
    void SetBusinessSubState(IBusinessSubState businessSubState);

    /// <summary>
    /// Method to implement logging or client notification on state changes
    /// </summary>
    void LogStates();

    /// <summary>
    /// The communication to the device was broken. Handle this event
    /// </summary>
    void HandleComDevClose();

    /// <summary>
    /// Restore the saved job state
    /// </summary>
    void RestoreSavedState();

    /// <summary>
    /// Save the state if it implements <see cref="IJobStateMachineState"/>
    /// </summary>
    /// <param name="stateMachineState">Current state</param>
    void SaveJobState(IStateMachineState stateMachineState);

}