// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.AdapterFactories;
using IpBackend.Bll.StateManagement.Configurators;

namespace IpBackend.Bll.Communication;

/// <summary>
/// Handles the TCP/IP channel between backend and IP device (client side) with state machine
/// </summary>
public class IpDeviceTcpIpClientStateMachineManager: IStateMachineDeviceManager
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;
    private readonly IOrderManagerFactory _orderManagerFactory;
    private ISocketProxyFactory _socketProxyFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="duplexIoFactory">Current factory for <see cref="IDuplexIo"/> instances</param>
    /// <param name="monitorLoggerFactoryFactory">Current factory for monitor logger factories</param>
    /// <param name="logDataFactory">Current log data factory</param>
    /// <param name="appLoggerFactory">Current logger proxy factory</param>
    /// <param name="appEventSourceFactory">Current factory for <see cref="IAppEventSource"/> instances</param>
    /// <param name="clientNotificationManager">Current client notification manager instance</param>
    /// <param name="appLoggerProxy">Current app logger</param>
    /// <param name="orderManagerFactory"></param>
    /// <param name="socketProxyFactory">Current socket factory</param>
    public IpDeviceTcpIpClientStateMachineManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy, 
        IOrderManagerFactory orderManagerFactory,
        ISocketProxyFactory socketProxyFactory)
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
        _socketProxyFactory = socketProxyFactory;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice? IpDevice { get; private set; }

    /// <summary>
    /// Current device
    /// </summary>
    public IStateMachineDevice? Device { get; private set; }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IStateMachineDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter{ get; private set; }

    /// <summary>
    /// Configure the device
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public void ConfigureDevice(string ipAddress, int port)
    {
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory = new TncpDataMessageProcessingPackageFactory();

        var configurator = new TcpIpClientStateMachineDeviceConfigurator(_duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, 
            _appEventSourceFactory, _clientNotificationManager, _appLoggerProxy, _socketProxyFactory);

        configurator.CreateMessagingConfig("IPDevice_TCPIP", ipAddress, port, messageProcessingPackageFactory);

        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new TncpIpDeviceTcpIpBusinessLogicAdapterFactory();
        configurator.CreateDevice(businessLogicAdapterFactory);

        // Order management
        configurator.ConfigureOrderManagement(_orderManagerFactory);

        // State management
        IStateMachineConfiguratorFactory stateMachineConfiguratorFactory = new TncpStateMachineConfiguratorFactory();
        configurator.ConfigureStateManagement(stateMachineConfiguratorFactory);

        var device = configurator.GetDevice();

        if (device is not IStateMachineDevice od)
        {
            throw new ArgumentException($"device does not implement {nameof(IStateMachineDevice)}");
        }

        if (device.DeviceBusinessLogicAdapter is not IStateMachineDeviceBusinessLogicAdapter dbla)
        {
            throw new ArgumentException($"device.DeviceBusinessLogicAdapter does not implement {nameof(IStateMachineDeviceBusinessLogicAdapter)}");
        }

        IpDevice = device;
        Device = od;
        DeviceBusinessLogicAdapter = dbla;
    }
}