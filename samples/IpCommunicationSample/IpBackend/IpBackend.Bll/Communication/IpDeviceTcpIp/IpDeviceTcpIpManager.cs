// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.BusinessLogic;
using IpCommunicationSample.Backend.Bll.StateManagement.Configurators;

namespace IpCommunicationSample.Backend.Bll.Communication.IpDeviceTcpIp;

public class IpDeviceTcpIpManager
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly ITcpIpListenerManager _tcpIpListenerManager;
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
    /// <param name="tcpIpListenerManager">Current TCP/IP listener manager</param>
    /// <param name="monitorLoggerFactoryFactory">Current factory for monitor logger factories</param>
    /// <param name="appLoggerProxy">Current app logger</param>
    /// <param name="orderManagerFactory">Current order manager factory</param>
    public IpDeviceTcpIpManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        ITcpIpListenerManager tcpIpListenerManager,
        IAppLoggerProxy appLoggerProxy,
        IOrderManagerFactory orderManagerFactory)
    {
        _duplexIoFactory = duplexIoFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
        _tcpIpListenerManager = tcpIpListenerManager;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _appLoggerProxy = appLoggerProxy;
        _orderManagerFactory = orderManagerFactory;
    }
    

    /// <summary>
    /// Configure the device
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public void ConfigureDevice(string ipAddress, int port)
    {
        IDeviceStateManagerFactory deviceStateManagerFactory = new TncpBackendDeviceStateManagerFactory();

        var configurator = new TcpIpClientStateMachineDeviceConfigurator(_duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLoggerProxy);

        configurator.CreateMessagingConfig("IPDevice_TCPIP", ipAddress, port);
        configurator.CreateDevice();
        configurator.ConfigureOrderManagement(_orderManagerFactory);

        IStateMachineConfiguratorFactory stateMachineConfiguratorFactory = new TncpStateMachineConfiguratorFactory();
        configurator.ConfigureStateManagement(deviceStateManagerFactory,  stateMachineConfiguratorFactory);

        var device = (IStateManagementDevice)configurator.GetDevice();

        Device = device;
        DeviceStateManager = device.DeviceStateManager;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IStateManagementDevice? Device { get; private set; }

    /// <summary>
    /// Current <see cref="IDeviceStateManager"/> instance
    /// </summary>
    public IDeviceStateManager? DeviceStateManager { get; private set; }
}