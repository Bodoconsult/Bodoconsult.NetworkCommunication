// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices.Configurators;


/// <summary>
/// Configurator for the communication to the IP device via TCP/IP (server side) withOUT state management
/// </summary>
public class TcpIpServerDeviceConfigurator: BaseIpDeviceConfigurator
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly ITcpIpListenerManager _tcpIpListenerManager;
    private readonly IAppLoggerProxy _appLoggerProxy;

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
    public TcpIpServerDeviceConfigurator(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        ITcpIpListenerManager tcpIpListenerManager,
        IAppLoggerProxy appLoggerProxy)
    {
        _duplexIoFactory = duplexIoFactory;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _clientNotificationManager = clientNotificationManager;
        _tcpIpListenerManager = tcpIpListenerManager;
        _appLoggerProxy = appLoggerProxy;
    }

    public override void CreateMessagingConfig(string loggerId, string ipAddress, int port,
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory)
    {
        if (loggerId.Length==0)
        {
            loggerId = $"{loggerId}_{port}";
        }

        DataMessagingConfig = new DefaultDataMessagingConfig();
        DataMessagingConfig.LoggerId = loggerId.Replace(" ","");
        DataMessagingConfig.AppLogger = _appLoggerProxy;
        DataMessagingConfig.MonitorLogger = CreateMonitorLogger(_monitorLoggerFactoryFactory, _appLoggerFactory, _logDataFactory, DataMessagingConfig.LoggerId);
        DataMessagingConfig.IpAddress = ipAddress;
        DataMessagingConfig.Port = port;
        DataMessagingConfig.IpProtocol = IpProtocolEnum.Tcp;
        DataMessagingConfig.IsServer = true;
        DataMessagingConfig.DataMessageProcessingPackage = new BtcpDataMessageProcessingPackage(DataMessagingConfig);
    }

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    public override void CreateDevice(IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        // Server
        var socketProxyFactory = new SocketProxyFactory(_tcpIpListenerManager);

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, _duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new SimpleDeviceFactory(_clientNotificationManager, commAdapterFactory);
        Device = factory.CreateInstance(DataMessagingConfig);

        var adapter = businessLogicAdapterFactory.CreateInstance(Device);
        Device.LoadDeviceBusinessLogicAdapter(adapter);
    }
}