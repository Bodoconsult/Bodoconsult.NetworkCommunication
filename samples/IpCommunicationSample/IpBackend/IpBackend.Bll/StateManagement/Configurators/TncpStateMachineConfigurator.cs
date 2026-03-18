// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Configurators;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.StateManagement.Configurators
{
    /// <summary>
    /// Configure a state management based on TNCP
    /// </summary>
    public class TncpStateMachineConfigurator : BaseStateMachineConfigurator
    {
        private readonly IBackendDeviceStateManager _deviceStateManager;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="deviceStateManager">Current device state manager</param>
        public TncpStateMachineConfigurator(IDeviceStateManager deviceStateManager) : base(deviceStateManager)
        {
            if (deviceStateManager is not IBackendDeviceStateManager dsm)
            {
                throw new ArgumentException($"deviceStateManager must have type {nameof(IBackendDeviceStateManager)}");
            }

            _deviceStateManager = dsm;
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
                CurrentContext = DeviceStateManager.Device,
                HandleAsyncMessageDelegate = _deviceStateManager.HandleAsyncMessageDelegate,
                HandleComDevCloseDelegate = _deviceStateManager.HandleComDevCloseDelegate,
                HandleErrorMessageDelegate = _deviceStateManager.HandleErrorMessageDelegate,
                HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
                OrderFinishedSucessfullyDelegate = _deviceStateManager.DeviceInitSuccessfully,
                OrderFinishedUnsucessfullyDelegate = _deviceStateManager.DeviceInitUnsuccessfully,
            };

            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceStopSnapshotStateBuilder()
        {
            var config = new JobStateConfiguration(DefaultStateNames.DeviceStopSnapshotState, new DeviceStopSnapshotStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                HandleAsyncMessageDelegate = _deviceStateManager.HandleAsyncMessageDelegate,
                HandleComDevCloseDelegate = _deviceStateManager.HandleComDevCloseDelegate,
                HandleErrorMessageDelegate = _deviceStateManager.HandleErrorMessageDelegate,
                HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
                OrderFinishedSucessfullyDelegate = _deviceStateManager.StopSnapshotSuccessfully,
                OrderFinishedUnsucessfullyDelegate = _deviceStateManager.StopSnapshotUnsuccessfully,
            };

            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceSnapshotStateBuilder()
        {
            var config = new NoActionStateConfiguration(DefaultStateNames.DeviceSnapshotState, new DeviceSnapshotStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
            };

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceStartSnapshotStateBuilder()
        {
            var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, new DeviceStartSnapshotStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                HandleAsyncMessageDelegate = _deviceStateManager.HandleAsyncMessageDelegate,
                HandleComDevCloseDelegate = _deviceStateManager.HandleComDevCloseDelegate,
                HandleErrorMessageDelegate = _deviceStateManager.HandleErrorMessageDelegate,
                HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
                OrderFinishedSucessfullyDelegate = _deviceStateManager.StartSnapshotSuccessfully,
                OrderFinishedUnsucessfullyDelegate = _deviceStateManager.StartSnapshotUnsuccessfully,
            };

            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceStopStreamingStateBuilder()
        {
            var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                HandleAsyncMessageDelegate = _deviceStateManager.HandleAsyncMessageDelegate,
                HandleComDevCloseDelegate = _deviceStateManager.HandleComDevCloseDelegate,
                HandleErrorMessageDelegate = _deviceStateManager.HandleErrorMessageDelegate,
                HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
                OrderFinishedSucessfullyDelegate = _deviceStateManager.StopStreamingSuccessfully,
                OrderFinishedUnsucessfullyDelegate = _deviceStateManager.StopStreamingUnsuccessfully,
            };

            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceStreamingStateBuilder()
        {
            var config = new NoActionStateConfiguration(DefaultStateNames.DeviceStreamingState, new DeviceStreamingStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
            };

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceStartStreamingStateBuilder()
        {
            var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                HandleAsyncMessageDelegate = _deviceStateManager.HandleAsyncMessageDelegate,
                HandleComDevCloseDelegate = _deviceStateManager.HandleComDevCloseDelegate,
                HandleErrorMessageDelegate = _deviceStateManager.HandleErrorMessageDelegate,
                HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
                OrderFinishedSucessfullyDelegate = _deviceStateManager.StartStreamingSuccessfully,
                OrderFinishedUnsucessfullyDelegate = _deviceStateManager.StartStreamingUnsuccessfully,
            };

            // Two orders required because of two telent commands to send
            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");
            config.OrderConfigurations.Add("NoAnswerTncpOrderConfiguration");

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceReadyStateBuilder()
        {
            var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState, new DeviceReadyStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
            };

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceOfflineStateBuilder()
        {
            var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, new DeviceOfflineStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                ExecuteActionForStateDelegate = DelegateHelper.ExecuteActionForStateDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
            };

            StateFactory.RegisterConfiguration(config);
        }

        private void AddDeviceOnlineStateBuilder()
        {
            var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState, new DeviceOnlineStateBuilder())
            {
                CurrentContext = DeviceStateManager.Device,
                ExecuteActionForStateDelegate = DelegateHelper.ExecuteActionForStateDelegate,
                PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
            };

            StateFactory.RegisterConfiguration(config);
        }
    }
}
