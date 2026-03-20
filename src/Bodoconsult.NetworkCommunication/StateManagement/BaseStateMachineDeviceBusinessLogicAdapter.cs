// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Base class for <see cref="IStateMachineDeviceBusinessLogicAdapter"/> implementations
/// </summary>
public abstract class BaseStateMachineDeviceBusinessLogicAdapter : IStateMachineDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    protected BaseStateMachineDeviceBusinessLogicAdapter(IStateManagementDevice device)
    {
        ArgumentNullException.ThrowIfNull(device.OrderManager);
        IpDevice = IpDevice = device;
        Device = device;
        OrderFactory = device.OrderManager.OrderFactory;
    }

    /// <summary>
    /// Current order factory
    /// </summary>
    protected IOrderFactory? OrderFactory;

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice IpDevice { get; }

    /// <summary>
    /// Current device
    /// </summary>
    public IStateManagementDevice Device { get; }

    /// <summary>
    /// Current state factory
    /// </summary>
    public IStateMachineStateFactory? StateFactory { get; private set; }

    /// <summary>
    /// Load the state factory
    /// </summary>
    /// <param name="stateFactory">Current state factory</param>
    public void LoadStateFactory(IStateMachineStateFactory stateFactory)
    {
        StateFactory = stateFactory;
    }

    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    public virtual void DefaultHandleComDevCloseDelegate(IStateMachineState state)
    {
        // Do nothing
    }

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    public virtual void DefaultHandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message)
    {
        // Do nothing
    }

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    public virtual MessageHandlingResult DefaultHandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage? message)
    {
        // Do nothing
        return MessageHandlingResultHelper.Success();
    }

    #region Static helper methods

    /// <summary>
    /// Create a state and register it with the state machine context for execution request
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="stateFactory">Current state factory</param>
    /// <param name="parameterSets">Required parametersets</param>
    /// <param name="stateName">Requested state name</param>
    /// <returns>State configuration</returns>
    /// <exception cref="ArgumentException">Thrown if configuration does not implement IJobStateConfiguration</exception>
    protected static void CreateAndRegisterState(IStateManagementDevice device, IStateMachineStateFactory stateFactory, List<IParameterSet> parameterSets, string stateName)
    {
        var state = stateFactory.CreateInstance(device, stateName, parameterSets);

        if (state is not IJobStateMachineState jobState)
        {
            throw new ArgumentException($"State does not implement {nameof(IJobStateMachineState)}");
        }

        // Register state for execution
        device.RegisterJobState(jobState);
    }

    /// <summary>
    /// Get the configuration for a state
    /// </summary>
    /// <param name="stateFactory">Current state factory</param>
    /// <param name="stateName">Requested state name</param>
    /// <returns>State configuration</returns>
    /// <exception cref="ArgumentException">Thrown if configuration does not implement IJobStateConfiguration</exception>
    protected static IJobStateConfiguration GetConfiguration(IStateMachineStateFactory stateFactory, string stateName)
    {
        var config = stateFactory.GetConfiguration(stateName);

        if (config is not IJobStateConfiguration jobConfig)
        {
            throw new ArgumentException($"Config must be {nameof(IJobStateConfiguration)}");
        }

        return jobConfig;
    }

    #endregion
}