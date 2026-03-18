// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;
using IpCommunicationSample.Backend.Bll.Communication.IpDeviceTcpIp;

namespace IpCommunicationSample.Backend.Bll.Communication.IpDeviceUdp;

public class IpDeviceUdpConfigurator : BaseIpDeviceConfigurator
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly ICentralClientNotificationManager _clientNotificationManager;

    public IpDeviceUdpConfigurator(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        ICentralClientNotificationManager clientNotificationManager)
    {
        _duplexIoFactory = duplexIoFactory;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _clientNotificationManager = clientNotificationManager;
    }

    public override void CreateMessagingConfig(string ipAddress, int port)
    {
        DataMessagingConfig = new DefaultDataMessagingConfig();
        DataMessagingConfig.IpAddress = ipAddress;
        DataMessagingConfig.Port = port;
        DataMessagingConfig.IpProtocol = IpProtocolEnum.Udp;
    }


    public override void CreateDevice()
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        // Client
        var socketProxyFactory = new ClientSocketProxyFactory();

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, _duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, _appEventSourceFactory, _clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new SimpleDeviceFactory(_clientNotificationManager, commAdapterFactory);
        Device = factory.CreateInstance(DataMessagingConfig);
    }
}