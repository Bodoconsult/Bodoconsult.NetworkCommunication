// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpClient.Bll.BusinessTransactions.AdapterFactories;

namespace IpClient.Bll.Communication;

/// <summary>
/// Handles the TCP/IP channel between client and backend (client side)
/// </summary>
public class BackendTcpIpClientManager : IOrderManagementDeviceManager
{
    private readonly IOrderIdGenerator _orderIdGenerator;
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;
    private readonly IOrderManagerFactory _orderManagerFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="duplexIoFactory">Current factory for <see cref="IDuplexIo"/> instances</param>
    /// <param name="logDataFactory">Current log data factory</param>
    /// <param name="appLoggerFactory">Current logger proxy factory</param>
    /// <param name="appEventSourceFactory">Current factory for <see cref="IAppEventSource"/> instances</param>
    /// <param name="clientNotificationManager">Current client notification manager instance</param>
    /// <param name="monitorLoggerFactoryFactory">Current factory for monitor logger factories</param>
    /// <param name="appLoggerProxy">Current app logger</param>
    /// <param name="orderManagerFactory">Current order manager factory</param>
    /// <param name="orderIdGenerator">Current order ID generator</param>
    public BackendTcpIpClientManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy,
        IOrderManagerFactory orderManagerFactory,
        IOrderIdGenerator orderIdGenerator)
    {
        _duplexIoFactory = duplexIoFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _appLoggerProxy = appLoggerProxy;
        _orderManagerFactory = orderManagerFactory;
        _orderIdGenerator = orderIdGenerator;
    }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IOrderManagementDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; private set; }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice? IpDevice { get; private set; }

    /// <summary>
    /// Configure the device
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public void ConfigureDevice(string ipAddress, int port)
    {
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory = new BtcpDataMessageProcessingPackageFactory();

        var configurator = new TcpIpClientOrderManagementDeviceConfigurator(_duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, _appEventSourceFactory, _clientNotificationManager, _appLoggerProxy);

        configurator.CreateMessagingConfig("Client_TCPIP", ipAddress, port, messageProcessingPackageFactory);

        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new BtcpBackendTcpIpBusinessLogicAdapterFactory(_orderIdGenerator);
        configurator.CreateDevice(businessLogicAdapterFactory);

        configurator.ConfigureOrderManagement(_orderManagerFactory);

        var device = configurator.GetDevice();

        if (device is not IOrderManagementDevice od)
        {
            throw new ArgumentNullException($"device does not implement {nameof(IOrderManagementDevice)}");
        }

        if (device.DeviceBusinessLogicAdapter is not IOrderManagementDeviceBusinessLogicAdapter dbla)
        {
            throw new ArgumentNullException($"device.DeviceBusinessLogicAdapter does not implement {nameof(IOrderManagementDeviceBusinessLogicAdapter)}");
        }

        IpDevice = device;
        Device = od;
        DeviceBusinessLogicAdapter = dbla;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IOrderManagementDevice? Device { get; private set; }
}