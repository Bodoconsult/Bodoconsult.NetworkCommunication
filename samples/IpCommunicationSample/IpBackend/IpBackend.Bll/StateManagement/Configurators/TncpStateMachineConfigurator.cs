// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Configurators;
using IpBackend.Bll.Interfaces;

namespace IpBackend.Bll.StateManagement.Configurators;

/// <summary>
/// Configure a state management based on TNCP
/// </summary>
public class TncpStateMachineConfigurator : BaseStateMachineConfigurator
{
    private readonly IIpDeviceTcpIpDeviceBusinessLogicAdapter _deviceBusinessLogicAdapter;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="deviceBusinessLogicAdapter">Current device state manager</param>
    public TncpStateMachineConfigurator(IStateMachineDeviceBusinessLogicAdapter deviceBusinessLogicAdapter) : base(deviceBusinessLogicAdapter)
    {
        if (deviceBusinessLogicAdapter is not IIpDeviceTcpIpDeviceBusinessLogicAdapter dsm)
        {
            throw new ArgumentException($"deviceBusinessLogicAdapter must have type {nameof(IIpDeviceTcpIpDeviceBusinessLogicAdapter)}");
        }

        _deviceBusinessLogicAdapter = dsm;
    }

    /// <summary>
    /// Configure the factory as required
    /// </summary>
    public override void ConfigureFactory()
    {
        AddDeviceOfflineStateBuilder();
        AddDeviceOnlineStateBuilder();
        AddDeviceInitStateBuilder();
        AddDeviceReadyStateBuilder();
        AddDeviceStartStreamingStateBuilder();
        AddDeviceStreamingStateBuilder();
        AddDeviceStopStreamingStateBuilder();
        AddDeviceStartSnapshotStateBuilder();
        AddDeviceSnapshotStateBuilder();
        AddDeviceStopSnapshotStateBuilder();
    }

    private void AddDeviceInitStateBuilder()
    {
        var config = new JobStateConfiguration(DefaultStateNames.DeviceInitState, new DeviceInitStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            HandleAsyncMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleAsyncMessage,
            HandleComDevCloseDelegate = _deviceBusinessLogicAdapter.DefaultHandleComDevClose,
            HandleErrorMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleErrorMessage,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = _deviceBusinessLogicAdapter.DeviceInitSuccessfully,
            OrderFinishedUnsucessfullyDelegate = _deviceBusinessLogicAdapter.DeviceInitUnsuccessfully,
        };

        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceStopSnapshotStateBuilder()
    {
        var config = new JobStateConfiguration(DefaultStateNames.DeviceStopSnapshotState, new DeviceStopSnapshotStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            HandleAsyncMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleAsyncMessage,
            HandleComDevCloseDelegate = _deviceBusinessLogicAdapter.DefaultHandleComDevClose,
            HandleErrorMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleErrorMessage,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = _deviceBusinessLogicAdapter.StopSnapshotSuccessfully,
            OrderFinishedUnsucessfullyDelegate = _deviceBusinessLogicAdapter.StopSnapshotUnsuccessfully,
        };

        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceSnapshotStateBuilder()
    {
        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceSnapshotState, new DeviceSnapshotStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
        };

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceStartSnapshotStateBuilder()
    {
        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, new DeviceStartSnapshotStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            HandleAsyncMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleAsyncMessage,
            HandleComDevCloseDelegate = _deviceBusinessLogicAdapter.DefaultHandleComDevClose,
            HandleErrorMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleErrorMessage,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = _deviceBusinessLogicAdapter.StartSnapshotSuccessfully,
            OrderFinishedUnsucessfullyDelegate = _deviceBusinessLogicAdapter.StartSnapshotUnsuccessfully,
        };

        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        // config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration"); // if order-command is coming

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceStopStreamingStateBuilder()
    {
        var config = new JobStateConfiguration(DefaultStateNames.DeviceStopStreamingState, new DeviceStopStreamingStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            HandleAsyncMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleAsyncMessage,
            HandleComDevCloseDelegate = _deviceBusinessLogicAdapter.DefaultHandleComDevClose,
            HandleErrorMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleErrorMessage,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = _deviceBusinessLogicAdapter.StopStreamingSuccessfully,
            OrderFinishedUnsucessfullyDelegate = _deviceBusinessLogicAdapter.StopStreamingUnsuccessfully,
        };

        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceStreamingStateBuilder()
    {
        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceStreamingState, new DeviceStreamingStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
        };

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceStartStreamingStateBuilder()
    {
        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            HandleAsyncMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleAsyncMessage,
            HandleComDevCloseDelegate = _deviceBusinessLogicAdapter.DefaultHandleComDevClose,
            HandleErrorMessageDelegate = _deviceBusinessLogicAdapter.DefaultHandleErrorMessage,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = _deviceBusinessLogicAdapter.StartStreamingSuccessfully,
            OrderFinishedUnsucessfullyDelegate = _deviceBusinessLogicAdapter.StartStreamingUnsuccessfully,
        };

        // Two orders required because of two telent commands to send
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
        // config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration"); // if order-command is coming

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceReadyStateBuilder()
    {
        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState, new DeviceReadyStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
        };

        StateFactory.RegisterConfiguration(config);
    }

    private void AddDeviceOfflineStateBuilder()
    {
        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, new DeviceOfflineStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            ExecuteActionForStateDelegate = DelegateHelper.DefaultExecuteActionForStateDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
        };

        StateFactory.RegisterConfiguration(config);
    }


    private void AddDeviceOnlineStateBuilder()
    {
        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState, new DeviceOnlineStateBuilder())
        {
            CurrentContext = DeviceBusinessLogicAdapter.Device,
            ExecuteActionForStateDelegate = StartCommunication,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
        };

        StateFactory.RegisterConfiguration(config);
    }


    private void StartCommunication(IOrderlessActionStateMachineState state)
    {
        ArgumentNullException.ThrowIfNull(state.CurrentContext.CommunicationAdapter);
        ArgumentNullException.ThrowIfNull(state.CurrentContext.StateMachineStateFactory);

        IStateMachineState? newState;

        try
        {
            state.CurrentContext.StartComm();

            //// Later call init to get config
            //var ps = new TncpParameterSet();

            //var newState = state.CurrentContext.StateMachineStateFactory.CreateInstance(state.CurrentContext, DefaultStateNames.DeviceInitState, [ps]);
            //state.CurrentContext.RequestState(newState);

            newState = state.CurrentContext.CreateStateInstance(state.CurrentContext.IsConnected ? 
                DefaultStateNames.DeviceReadyState : 
                DefaultStateNames.DeviceOfflineState);

            state.CurrentContext.RequestState(newState);
        }
        catch (Exception e)
        {
            Debug.Print(e.ToString());
            newState = state.CurrentContext.CreateStateInstance(DefaultStateNames.DeviceOfflineState);
            state.CurrentContext.RequestState(newState);
        }
    }
}