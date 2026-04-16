// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using IpBackend.Bll.App;
using IpBackend.Bll.Interfaces;

namespace IpBackend.Bll.BusinessLogic.Adapters;

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
        // Do nothing
    }

    /// <summary>
    /// Stopping snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Starting snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotSuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Starting snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Starting streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Stopping streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Init device was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitSuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Init device was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitUnsuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    #endregion

    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartStreamingState(IBusinessTransactionRequestData request)
    {
        try
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
                CreateStartOrders(jobConfig, index, parameterSets, OrderFactory, "continious");
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1000,
                Message = "Starting streaming failed",
                ExceptionMessage = e.ToString()
            };
        }
    }

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request)
    {
        try
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
                CreateStartOrders(jobConfig, index, parameterSets, OrderFactory, "snapshot");
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1002,
                Message = "Starting snapshot failed",
                ExceptionMessage = e.ToString()
            };
        }
    }

    private void CreateStartOrders(IJobStateConfiguration jobConfig, int index, List<IParameterSet> parameterSets,
        IOrderFactory orderFactory, string mode)
    {
        var orderConfigName = jobConfig.OrderConfigurations[index];
        var orderConfig = orderFactory.GetConfiguration(orderConfigName);

        

        ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
        ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

        var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

        switch (index)
        {
            //case 6:
            //    ps.TelnetCommand = "set,snapshot,4,4";   // 
            //    break;
            case 5:
                ps.TelnetCommand = "set,status,start";   // 
                break;
            case 4:
                ps.TelnetCommand = $"set,stream,mode,{mode}";   // 
                break;
            case 3:
                var appParams = (ThreeNetworkDevicesAppStartParameter)Globals.Instance.AppStartParameter;
                ps.TelnetCommand = $"set,connection,{IpHelper.GetLocalIpAddress()},{appParams.Port}";   // 
                break;
            case 2:
                ps.TelnetCommand = "set,snapshot,4,4";   // 
                break;
            //case 1:
            default:
                ps.TelnetCommand = "set,stream,number,1";   // One channel
                break;
        }

        parameterSets.Add(ps);
    }

    /// <summary>
    /// Request a stop streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopStreamingState(IBusinessTransactionRequestData request)
    {
        try
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

                ps.TelnetCommand = ps.TelnetCommand = "set,status,stop"; ;

                // ps.TelnetCommand = "StopStreaming";

                parameterSets.Add(ps);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1001,
                Message = "Stopping streaming failed",
                ExceptionMessage = e.ToString()
            };
        }

    }

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request)
    {
        try
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

                ps.TelnetCommand = "set,status,stop";

                parameterSets.Add(ps);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, parameterSets, stateName);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1003,
                Message = "Stopping snapshot failed",
                ExceptionMessage = e.ToString()
            };
        }

    }

    #endregion
}