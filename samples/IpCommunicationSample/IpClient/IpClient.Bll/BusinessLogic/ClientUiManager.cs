// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpClient.Bll.BusinessTransactions.Providers;
using IpClient.Bll.Communication;
using IpClient.Bll.Interfaces;

namespace IpClient.Bll.BusinessLogic;

public class ClientUiManager: IClientUiManager
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
    private readonly IAppBenchProxy _appBenchProxy;
    private readonly IOrderReceiverFactory _orderReceiverFactory;
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory;
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory;
    private readonly IOrderPipelineFactory _orderPipelineFactory;
    private readonly IOrderIdGenerator _orderIdGenerator;
    private readonly IBusinessTransactionManager _businessTransactionManager;
    private readonly ISocketProxyFactory _socketProxyFactory;
    private readonly IUiStateHandler _uiStateHandler;

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
    /// <param name="dateService">Current app date service</param>
    /// <param name="syncOrderManager">Current sync execution manager</param>
    /// <param name="appBenchProxy">Current bench logger</param>
    /// <param name="orderReceiverFactory">Current order receiver factory</param>
    /// <param name="requestProcessorFactoryFactory">Current factory for request processor factories</param>
    /// <param name="requestStepProcessorFactoryFactory">Current factory for request step processor factories</param>
    /// <param name="orderPipelineFactory">Current factory for order pipelines</param>
    /// <param name="orderIdGenerator">Current order ID generator</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    /// <param name="socketProxyFactory">Current socket factory</param>
    /// <param name="uiStateHandler">Current UI state handler</param>
    public ClientUiManager(IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLogger,
        ISendPacketProcessFactory sendPacketProcessFactory,
        IAppDateService dateService,
        ISyncOrderManager syncOrderManager,
        IAppBenchProxy appBenchProxy,
        IOrderReceiverFactory orderReceiverFactory,
        IRequestProcessorFactoryFactory requestProcessorFactoryFactory,
        IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory,
        IOrderPipelineFactory orderPipelineFactory,
        IOrderIdGenerator orderIdGenerator,
        IBusinessTransactionManager businessTransactionManager,
        ISocketProxyFactory socketProxyFactory,
        IUiStateHandler uiStateHandler
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
        _dateService = dateService;
        _syncOrderManager = syncOrderManager;
        _appBenchProxy = appBenchProxy;
        _orderReceiverFactory = orderReceiverFactory;
        _requestProcessorFactoryFactory = requestProcessorFactoryFactory;
        _requestStepProcessorFactoryFactory = requestStepProcessorFactoryFactory;
        _orderPipelineFactory = orderPipelineFactory;
        _orderIdGenerator = orderIdGenerator;
        _businessTransactionManager = businessTransactionManager;
        _socketProxyFactory = socketProxyFactory;
        _uiStateHandler = uiStateHandler;
    }

    /// <summary>
    /// Current IP config of the backend for TCP/IP
    /// </summary>
    public IpConfig? BackendTcpIpConfig { get; set; }

    /// <summary>
    /// Represents the TCP/IP communication with the IP device
    /// </summary>
    public IOrderManagementDeviceManager? BackendTcpIp { get; private set; }

    /// <summary>
    /// Load the comm via TCP/IP to the backend
    /// </summary>
    public void LoadBackendTcpIp()
    {
        if (BackendTcpIpConfig == null)
        {
            throw new ArgumentNullException(nameof(BackendTcpIpConfig));
        }

        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        IOrderProcessorFactory orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderFactory orderFactory = new BtcpOrderFactory(_orderIdGenerator);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory,
            _requestProcessorFactoryFactory, _orderPipelineFactory, orderFactory);
        var m = new BackendTcpIpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, orderManagerFactory, _orderIdGenerator, _socketProxyFactory,
            _uiStateHandler);

        m.ConfigureDevice(BackendTcpIpConfig.Value.IpAddress, BackendTcpIpConfig.Value.Port);

        BackendTcpIp = m;
    }

    /// <summary>
    /// Load the business transactions required for the app
    /// </summary>
    public void LoadBusinessTransactions()
    {
        ArgumentNullException.ThrowIfNull(BackendTcpIp);

        var adapter = (IBackendTcpIpBusinessLogicAdapter?)BackendTcpIp.DeviceBusinessLogicAdapter;

        ArgumentNullException.ThrowIfNull(adapter);

        IBusinessTransactionProvider provider = new BackendTcpIpBusinessTransactionProvider(adapter);
        _businessTransactionManager.AddProvider(provider);
    }

    /// <summary>
    /// Start the communication with the TCP/IP backend
    /// </summary>
    public void StartBackendTcpIpCommunication()
    {
        ArgumentNullException.ThrowIfNull(BackendTcpIp);
        ArgumentNullException.ThrowIfNull(BackendTcpIp.IpDevice);
        ArgumentNullException.ThrowIfNull(BackendTcpIp.Device);
        BackendTcpIp.IpDevice.StartComm();
        BackendTcpIp.Device.Start();
    }
}