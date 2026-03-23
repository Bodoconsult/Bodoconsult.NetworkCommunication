// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current implementation of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for the backend
/// </summary>
public class TncpIpDeviceTcpIpBusinessLogicAdapter : BaseStateMachineDeviceBusinessLogicAdapter, IIpDeviceTcpIpDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TncpIpDeviceTcpIpBusinessLogicAdapter(IStateMachineDevice device) : base(device)
    { }

    #region Device order handling

    /// <summary>
    /// Stopping snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Init device was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Init device was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStartStreamingState(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
        ArgumentNullException.ThrowIfNull(OrderFactory);

        const string stateName = DefaultStateNames.DeviceStartStreamingState;

        // Get the state config
        var jobConfig = GetConfiguration(StateFactory, stateName);

        // Get the required parametersets now
        var parameterSets = new List<IParameterSet>();

        for (var index = 0; index < jobConfig.OrderConfigurations.Count; index++)
        {
            var orderConfigName = jobConfig.OrderConfigurations[index];
            var orderConfig = OrderFactory.GetConfiguration(orderConfigName);

            ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
            ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

            var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

            ps.TelnetCommand = index == 0 ? "StartStreaming1" : "StartStreaming2";

            parameterSets.Add(ps);
        }

        // Now create the state
        CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);
    }

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
        ArgumentNullException.ThrowIfNull(OrderFactory);

        const string stateName = DefaultStateNames.DeviceStartSnapshotState;

        // Get the state config
        var jobConfig = GetConfiguration(StateFactory, stateName);

        // Get the required parametersets now
        var parameterSets = new List<IParameterSet>();

        for (var index = 0; index < jobConfig.OrderConfigurations.Count; index++)
        {
            var orderConfigName = jobConfig.OrderConfigurations[index];
            var orderConfig = OrderFactory.GetConfiguration(orderConfigName);

            ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
            ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

            var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

            ps.TelnetCommand = index == 0 ? "StartSnapshot1" : "StartSnapshot2";

            parameterSets.Add(ps);
        }

        // Now create the state
        CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);
    }

    /// <summary>
    /// Request a stop streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStopStreamingState(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
        ArgumentNullException.ThrowIfNull(OrderFactory);

        const string stateName = DefaultStateNames.DeviceStopStreamingState;

        // Get the state config
        var jobConfig = GetConfiguration(StateFactory, stateName);

        // Get the required parametersets now
        var parameterSets = new List<IParameterSet>();

        foreach (var orderConfigName in jobConfig.OrderConfigurations)
        {
            var orderConfig = OrderFactory.GetConfiguration(orderConfigName);

            ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
            ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

            var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

            ps.TelnetCommand = "StopStreaming";

            parameterSets.Add(ps);
        }

        // Now create the state
        CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);
    }

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
        ArgumentNullException.ThrowIfNull(OrderFactory);

        const string stateName = DefaultStateNames.DeviceStopSnapshotState;

        // Get the state config
        var jobConfig = GetConfiguration(StateFactory, stateName);

        // Get the required parametersets now
        var parameterSets = new List<IParameterSet>();

        foreach (var orderConfigName in jobConfig.OrderConfigurations)
        {
            var orderConfig = OrderFactory.GetConfiguration(orderConfigName);

            ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
            ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

            var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

            ps.TelnetCommand = "StopSnapshot";

            parameterSets.Add(ps);
        }

        // Now create the state
        CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);
    }

    #endregion
}