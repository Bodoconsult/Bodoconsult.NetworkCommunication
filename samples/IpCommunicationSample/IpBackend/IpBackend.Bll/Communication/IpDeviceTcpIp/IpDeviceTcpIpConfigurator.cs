// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;

namespace IpCommunicationSample.Backend.Bll.Communication.IpDeviceTcpIp;

public class IpDeviceTcpIpConfigurator : BaseIpDeviceConfigurator
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly ITcpIpListenerManager _tcpIpListenerManager;

    public IpDeviceTcpIpConfigurator(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        ITcpIpListenerManager tcpIpListenerManager)
    {
        _duplexIoFactory = duplexIoFactory;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _clientNotificationManager = clientNotificationManager;
        _tcpIpListenerManager = tcpIpListenerManager;
    }

    public override void CreateMessagingConfig(string ipAddress, int port)
    {
        DataMessagingConfig = new DefaultDataMessagingConfig();
        DataMessagingConfig.IpAddress = ipAddress;
        DataMessagingConfig.Port = port;
        DataMessagingConfig.IpProtocol = IpProtocolEnum.Tcp;
    }

    public override void CreateDevice()
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        // Server
        var socketProxyFactory = new ServerSocketProxyFactory(_tcpIpListenerManager);

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, _duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, _appEventSourceFactory, _clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new BasicManagementDeviceFactory(_clientNotificationManager, commAdapterFactory);
        Device = factory.CreateInstance(DataMessagingConfig);
    }
}