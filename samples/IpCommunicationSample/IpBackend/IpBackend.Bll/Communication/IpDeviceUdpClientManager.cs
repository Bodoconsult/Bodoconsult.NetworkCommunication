// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSample.Backend.Bll.Communication;

/// <summary>
/// Handles the TCP/IP channel between backend and IP device
/// </summary>
public class IpDeviceUdpClientManager : IDeviceManager
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;
    
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
    public IpDeviceUdpClientManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy)
    {
        _duplexIoFactory = duplexIoFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _appLoggerProxy = appLoggerProxy;
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
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory = new TncpDataMessageProcessingPackageFactory();

        var configurator = new UdpClientDeviceConfigurator(_duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, _appEventSourceFactory, _clientNotificationManager, _appLoggerProxy);

        configurator.CreateMessagingConfig("IPDevice_UDP", ipAddress, port, messageProcessingPackageFactory);

        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new SdcpIpDeviceUdpBusinessLogicAdapterFactory();
        configurator.CreateDevice(businessLogicAdapterFactory);

        var device = configurator.GetDevice();

        if (device.DeviceBusinessLogicAdapter is not ISimpleDeviceBusinessLogicAdapter dbla)
        {
            throw new ArgumentNullException($"device.DeviceBusinessLogicAdapter does not implement {nameof(ISimpleDeviceBusinessLogicAdapter)}");
        }

        IpDevice = device;
        DeviceBusinessLogicAdapter= dbla;
    }
}