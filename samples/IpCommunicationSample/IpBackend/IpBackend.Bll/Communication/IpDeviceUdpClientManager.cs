// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.App;
using IpBackend.Bll.BusinessLogic.AdapterFactories;

namespace IpBackend.Bll.Communication;

/// <summary>
/// Handles the TCP/IP channel between backend and IP device
/// </summary>
public class IpDeviceUdpClientManager : ISimpleDeviceManager
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;
    private readonly ISocketProxyFactory _socketProxyFactory;
    private readonly IAppGlobals _appGlobals;

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
    /// <param name="socketProxyFactory">Current socket factory</param>
    /// <param name="appGlobals">Current app globals</param>
    public IpDeviceUdpClientManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy,
        ISocketProxyFactory socketProxyFactory,
        IAppGlobals appGlobals)
    {
        _duplexIoFactory = duplexIoFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _appLoggerProxy = appLoggerProxy;
        _socketProxyFactory = socketProxyFactory;
        _appGlobals = appGlobals;
    }

    /// <summary>
    /// Current <see cref="ISimpleDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public ISimpleDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter{ get; private set; }

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
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory = new SfxpLoggedSortableDataMessageProcessingPackageFactory();

        var configurator = new UdpClientDeviceConfigurator(_duplexIoFactory, _monitorLoggerFactoryFactory,
            _logDataFactory, _appLoggerFactory, _appEventSourceFactory,
            _clientNotificationManager, _appLoggerProxy, _socketProxyFactory, _appGlobals); 

        configurator.CreateMessagingConfig("IPDevice_UDP: ", ipAddress, port, messageProcessingPackageFactory);

        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new SfxpIpDeviceUdpBusinessLogicAdapterFactory();
        configurator.CreateDevice(businessLogicAdapterFactory);

        var device = configurator.GetDevice();

        device.DataMessagingConfig.DataLoggingFileName = $"IPDevice_{ipAddress.Replace(".", "_", StringComparison.InvariantCultureIgnoreCase)}";
        device.DataMessagingConfig.DataLoggingPath = Globals.Instance.DataPath;

        if (device.DeviceBusinessLogicAdapter is not ISimpleDeviceBusinessLogicAdapter dbla)
        {
            throw new ArgumentException($"device.DeviceBusinessLogicAdapter does not implement {nameof(ISimpleDeviceBusinessLogicAdapter)}");
        }

        IpDevice = device;
        DeviceBusinessLogicAdapter= dbla;
    }
}