// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessTransactions.Providers;
using IpBackend.Bll.Communication;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpBackend.Bll.BusinessLogic;

/// <summary>
/// Current implementation of <see cref="IBackendManager"/> handling the full back comm to IP device and client
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
    private readonly IAppBenchProxy _appBenchProxy;
    private readonly IOrderReceiverFactory _orderReceiverFactory;
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory;
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory;
    private readonly IOrderPipelineFactory _orderPipelineFactory;
    private readonly IOrderIdGenerator _orderIdGenerator;
    private readonly IBusinessTransactionManager _businessTransactionManager;
    private readonly ISocketProxyFactory _socketProxyFactory;
    private readonly IAppGlobals _appGlobals;

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
    /// <param name="appGlobals">Current app globals</param>
    public BackendManager(IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
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
        IAppGlobals appGlobals
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
        _appGlobals = appGlobals;

        _appLogger.LogWarning("Hallo");
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
    public ISimpleDeviceManager? Client { get; private set; }

    /// <summary>
    /// Represents the TCP/IP communication with the IP device
    /// </summary>
    public IStateMachineDeviceManager? IpDeviceTcpIp { get; private set; }

    /// <summary>
    /// Represents the UDP communication with the IP device
    /// </summary>
    public ISimpleDeviceManager? IpDeviceUdp { get; private set; }

    /// <summary>
    /// Load the comm via TCP/IP to the device
    /// </summary>
    public void LoadIpDeviceTcpIp()
    {
        if (IpDeviceTcpIpConfig == null)
        {
            throw new ArgumentNullException(nameof(IpDeviceTcpIpConfig));
        }

        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        IOrderProcessorFactory orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderFactory orderFactory = new TncpOrderFactory(_orderIdGenerator);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory,
            _requestProcessorFactoryFactory, _orderPipelineFactory, orderFactory);
        var m = new IpDeviceTcpIpClientStateMachineManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, orderManagerFactory, _socketProxyFactory);

        m.ConfigureDevice(IpDeviceTcpIpConfig.Value.IpAddress, IpDeviceTcpIpConfig.Value.Port);

        IpDeviceTcpIp = m;

        ArgumentNullException.ThrowIfNull(IpDeviceTcpIp);
        ArgumentNullException.ThrowIfNull(IpDeviceTcpIp.Device);
        ArgumentNullException.ThrowIfNull(IpDeviceUdp);
        ArgumentNullException.ThrowIfNull(IpDeviceUdp.IpDevice);

        IpDeviceTcpIp.Device.ConnectedDevices.Add(IpDeviceUdp.IpDevice);

    }

    /// <summary>
    /// Load the comm via UDP to the device
    /// </summary>
    public void LoadIpDeviceUdp()
    {
        if (IpDeviceUdpConfig == null)
        {
            throw new ArgumentNullException(nameof(IpDeviceUdpConfig));
        }

        var duplexIoFactory = new UdpDatagramIpDuplexIoFactory(_sendPacketProcessFactory);

        var m = new IpDeviceUdpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _socketProxyFactory, _appGlobals);

        m.ConfigureDevice(IpDeviceUdpConfig.Value.IpAddress, IpDeviceUdpConfig.Value.Port);

        IpDeviceUdp = m;
    }

    /// <summary>
    /// Load the client
    /// </summary>
    public void LoadClient()
    {
        if (ClientTcpIpConfig == null)
        {
            throw new ArgumentNullException(nameof(ClientTcpIpConfig));
        }

        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var m = new ClientTcpIpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _businessTransactionManager, _socketProxyFactory);

        m.ConfigureDevice(ClientTcpIpConfig.Value.IpAddress, ClientTcpIpConfig.Value.Port);

        Client = m;
    }

    /// <summary>
    /// Load the business transactions required for the app
    /// </summary>
    public void LoadBusinessTransactions()
    {
        ArgumentNullException.ThrowIfNull(IpDeviceTcpIp);
        ArgumentNullException.ThrowIfNull(IpDeviceUdp);

        if (IpDeviceUdpConfig == null)
        {
            throw new ArgumentNullException(nameof(IpDeviceUdpConfig));
        }

        var adapter = (IIpDeviceTcpIpDeviceBusinessLogicAdapter?)IpDeviceTcpIp.DeviceBusinessLogicAdapter;
        var adapter2 = (IIpDeviceUdpDeviceBusinessLogicAdapter?)IpDeviceUdp.DeviceBusinessLogicAdapter;

        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(adapter2);

        adapter.UdpPort = IpDeviceUdpConfig.Value.Port;

        IBusinessTransactionProvider provider = new IpDeviceTcpIpBusinessTransactionProvider(adapter);
        _businessTransactionManager.AddProvider(provider);

        provider = new IpDeviceUdpBusinessTransactionProvider(adapter2);
        _businessTransactionManager.AddProvider(provider);
    }

    /// <summary>
    /// Start the communication with the UDP device
    /// </summary>
    public void StartIpDeviceUdpCommunication()
    {
        ArgumentNullException.ThrowIfNull(IpDeviceUdp?.IpDevice);

        IpDeviceUdp.IpDevice.DataMessagingConfig.SendHelloMessageDelegate = SendHelloMessageDelegate;
        IpDeviceUdp.IpDevice.StartComm();
    }

    /// <summary>
    /// Send a hello message to the UDP device on ComDevInit
    /// </summary>
    /// <returns>True if the emssage was sent successfully else false</returns>
    private bool SendHelloMessageDelegate()
    {
        // Send hello
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.SendClientHello
        };

        var reply = _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);

        if (reply.ErrorCode == 0)
        {
            return true;
        }

        ArgumentNullException.ThrowIfNull(IpDeviceUdp?.IpDevice);
        IpDeviceUdp.IpDevice.DataMessagingConfig.AppLogger.LogError("HELLO message could NOT be sent");
        return false;
    }

    /// <summary>
    /// Start the communication with the IP device via UDP
    /// </summary>
    public void StartIpDeviceTcpIpCommunication()
    {
        ArgumentNullException.ThrowIfNull(IpDeviceTcpIp?.Device?.StateMachineStateFactory);

        IpDeviceTcpIp.Device.Start();

        var newState = IpDeviceTcpIp.Device.CreateStateInstance(DefaultStateNames.DeviceOfflineState);
        IpDeviceTcpIp.Device.RequestState(newState);

        //ArgumentNullException.ThrowIfNull(IpDeviceTcpIp?.IpDevice);
        //IpDeviceTcpIp.IpDevice.StartComm();
    }

    /// <summary>
    /// Start the communication with the client
    /// </summary>
    public void StartClientCommunication()
    {
        ArgumentNullException.ThrowIfNull(Client?.IpDevice);
        Client.IpDevice.StartComm();
    }
}