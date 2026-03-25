// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.Communication;
using IpCommunicationSample.Backend.Bll.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories
{
    /// <summary>
    /// Current implementation of <see cref="IBackendManager"/>
    /// </summary>
    public class BackendManager : IBackendManager
    {
        private readonly IAppDateService _dateService;
        private readonly ISyncOrderManager _syncOrderManager;
        private readonly IAppEventSourceFactory _appEventSourceFactory;
        private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
        private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
        private readonly ILogDataFactory _logDataFactory;
        private readonly IAppLoggerProxyFactory _appLoggerFactory;
        private readonly IAppLoggerProxy _appLogger;
        private readonly ISendPacketProcessFactory _sendPacketProcessFactory;
        private readonly ITcpIpListenerManager _tcpIpListenerManager;
        private readonly IAppBenchProxy _appBenchProxy;
        private readonly IOrderReceiverFactory _orderReceiverFactory;
        private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory;
        private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory;
        private readonly IOrderPipelineFactory _orderPipelineFactory;
        private readonly IOrderIdGenerator _orderIdGenerator;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="logDataFactory">Current log data factory</param>
        /// <param name="appLoggerFactory">Current logger proxy factory</param>
        /// <param name="appEventSourceFactory">Current factory for <see cref="IAppEventSource"/> instances</param>
        /// <param name="clientNotificationManager">Current client notification manager instance</param>
        /// <param name="monitorLoggerFactoryFactory">Current factory for monitor logger factories</param>
        /// <param name="appLogger">Current app logger</param>
        /// <param name="sendPacketProcessFactory">Current send packet process factory</param>
        /// <param name="tcpIpListenerManager">Current TCP/IP listener manager</param>
        /// <param name="dateService">Current app date service</param>
        /// <param name="syncOrderManager">Current sync execution manager</param>
        /// <param name="appBenchProxy">Current bench logger</param>
        /// <param name="orderReceiverFactory">Current order receiver factory</param>
        /// <param name="requestProcessorFactoryFactory">Current factory for request processor factories</param>
        /// <param name="requestStepProcessorFactoryFactory">Current factory for request step processor factories</param>
        /// <param name="orderPipelineFactory">Current factory for order pipelines</param>
        /// <param name="orderIdGenerator">Current order ID generator</param>
        public BackendManager(IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
            ILogDataFactory logDataFactory,
            IAppLoggerProxyFactory appLoggerFactory,
            IAppEventSourceFactory appEventSourceFactory,
            IOrderManagementClientNotificationManager clientNotificationManager,
            IAppLoggerProxy appLogger,
            ISendPacketProcessFactory sendPacketProcessFactory,
            ITcpIpListenerManager tcpIpListenerManager,
            IAppDateService dateService,
            ISyncOrderManager syncOrderManager,
            IAppBenchProxy appBenchProxy,
            IOrderReceiverFactory orderReceiverFactory,
            IRequestProcessorFactoryFactory requestProcessorFactoryFactory,
            IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory,
            IOrderPipelineFactory orderPipelineFactory,
            IOrderIdGenerator orderIdGenerator
            )
        {
            _appEventSourceFactory = appEventSourceFactory;
            _clientNotificationManager = clientNotificationManager;
            _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
            _appLoggerFactory = appLoggerFactory;
            _appEventSourceFactory = appEventSourceFactory;
            _logDataFactory = logDataFactory;
            _appLogger = appLogger;
            _sendPacketProcessFactory = sendPacketProcessFactory;
            _tcpIpListenerManager = tcpIpListenerManager;
            _dateService = dateService;
            _syncOrderManager = syncOrderManager;
            _appBenchProxy = appBenchProxy;
            _orderReceiverFactory = orderReceiverFactory;
            _requestProcessorFactoryFactory = requestProcessorFactoryFactory;
            _requestStepProcessorFactoryFactory = requestStepProcessorFactoryFactory;
            _orderPipelineFactory = orderPipelineFactory;
            _orderIdGenerator = orderIdGenerator;
        }

        /// <summary>
        /// Current IP config of the IP device for TCP/IP
        /// </summary>
        public IpConfig? IpDeviceTcpIpConfig { get; set; }

        /// <summary>
        /// Current IP config of the IP device for UDP
        /// </summary>
        public IpConfig? IpDeviceUdpConfig { get; set; }

        /// <summary>
        /// Current IP config of the client for TCP/IP
        /// </summary>
        public IpConfig? ClientTcpIpConfig { get; set; }

        /// <summary>
        /// Represents the TCP/IP communication with the client
        /// </summary>
        public IIpDevice? Client { get; private set; }

        /// <summary>
        /// Represents the TCP/IP communication with the IP device
        /// </summary>
        public IStateMachineDevice? IpDeviceTcpIp { get; private set; }

        /// <summary>
        /// Represents the UDP communication with the IP device
        /// </summary>
        public IOrderManagementDevice? IpDeviceUdp { get; private set; }

        /// <summary>
        /// Load the comm via TCP/IP to the device
        /// </summary>
        public void LoadIpDeviceTcpIp()
        {
            ArgumentNullException.ThrowIfNull(IpDeviceTcpIpConfig);

            var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

            IOrderProcessorFactory orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
            IOrderFactory orderFactory = new TncpOrderFactory(_orderIdGenerator) ;
            IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory, 
                    _requestProcessorFactoryFactory, _orderPipelineFactory, orderFactory);
            var m = new IpDeviceTcpIpClientStateMachineManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
                _appEventSourceFactory, _clientNotificationManager, _appLogger, orderManagerFactory);

            m.ConfigureDevice(IpDeviceTcpIpConfig.Value.IpAddress, IpDeviceTcpIpConfig.Value.Port);

            IpDeviceTcpIp = m.Device;
        }

        /// <summary>
        /// Load the comm via UDP to the device
        /// </summary>
        public void LoadIpDeviceUdp()
        {
            ArgumentNullException.ThrowIfNull(IpDeviceUdpConfig);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Load the client
        /// </summary>
        public void LoadClient()
        {
            ArgumentNullException.ThrowIfNull(ClientTcpIpConfig);

            var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

            var m = new ClientTcpIpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
                _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger);

            m.ConfigureDevice(ClientTcpIpConfig.Value.IpAddress, ClientTcpIpConfig.Value.Port);

            Client = m.IpDevice;
        }
    }
}
