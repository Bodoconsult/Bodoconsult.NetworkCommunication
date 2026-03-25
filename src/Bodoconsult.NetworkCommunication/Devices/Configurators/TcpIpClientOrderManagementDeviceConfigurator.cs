// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices.Configurators;

/// <summary>
/// Configurator for the communication to the IP device via TCP/IP (client side) with order management but no state management
/// </summary>
public class TcpIpClientOrderManagementDeviceConfigurator : BaseIpDeviceConfigurator
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;

    private IOrderManagementDevice? _orderManagementDevice;

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
    public TcpIpClientOrderManagementDeviceConfigurator(IDuplexIoFactory duplexIoFactory,
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
    /// Create the basic data messaging config
    /// </summary>
    /// <param name="loggerId">Logger ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <param name="messageProcessingPackageFactory">Current data messaging package factory</param>
    public override void CreateMessagingConfig(string loggerId, string ipAddress, int port,
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory)
    {
        if (loggerId.Length == 0)
        {
            loggerId = $"{loggerId}_{port}";
        }

        DataMessagingConfig = new DefaultDataMessagingConfig();
        DataMessagingConfig.LoggerId = loggerId.Replace(" ", "");
        DataMessagingConfig.AppLogger = _appLoggerProxy;
        DataMessagingConfig.MonitorLogger = CreateMonitorLogger(_monitorLoggerFactoryFactory, _appLoggerFactory, _logDataFactory, DataMessagingConfig.LoggerId);
        DataMessagingConfig.IpAddress = ipAddress;
        DataMessagingConfig.Port = port;
        DataMessagingConfig.IpProtocol = IpProtocolEnum.Tcp;
        DataMessagingConfig.IsServer = false;
        DataMessagingConfig.DataMessageProcessingPackage = messageProcessingPackageFactory.CreateInstance(DataMessagingConfig);
    }

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    public override void CreateDevice(IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        // Server
        var socketProxyFactory = new SocketProxyFactory(null);

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, _duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new BasicOrderManagementDeviceFactory(_clientNotificationManager, commAdapterFactory);
        Device = factory.CreateInstance(DataMessagingConfig);

        if (Device is not IOrderManagementDevice smd)
        {
            throw new ArgumentException($"Device must implement {nameof(IOrderManagementDevice)}");
        }

        var dsm = businessLogicAdapterFactory.CreateInstance(Device);

        if (dsm is not IOrderManagementDeviceBusinessLogicAdapter adapter)
        {
            throw new ArgumentException($"dsm is not implementing {nameof(IOrderManagementDeviceBusinessLogicAdapter)}");
        }

        Device.LoadDeviceBusinessLogicAdapter(adapter);

        _orderManagementDevice = smd;
    }

    /// <summary>
    /// Configure the order management
    /// </summary>
    /// <param name="orderManagerFactory">Current factory for <see cref="IOrderManager"/> instances</param>
    public override void ConfigureOrderManagement(IOrderManagerFactory orderManagerFactory)
    {
        ArgumentNullException.ThrowIfNull(_orderManagementDevice);
        ArgumentNullException.ThrowIfNull(_orderManagementDevice.OrderManagementDeviceBusinessLogicAdapter);

        // Order management
        var om = orderManagerFactory.CreateInstance(_orderManagementDevice);
        _orderManagementDevice.LoadDeviceOrderManager(om);

        _orderManagementDevice.OrderManagementDeviceBusinessLogicAdapter.LoadOrderFactory(om.OrderFactory);
    }


    /// <summary>
    /// Configure the order management and if necessary the state management. Important: store state factory instance to device and config
    /// </summary>
    /// <param name="stateMachineConfiguratorFactory">Current state machine configurator factory</param>
    public override void ConfigureStateManagement(IStateMachineConfiguratorFactory stateMachineConfiguratorFactory)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(_orderManagementDevice);

        if (Device.DeviceBusinessLogicAdapter is not IStateMachineDeviceBusinessLogicAdapter adapter)
        {
            throw new ArgumentException($"Device.DeviceBusinessLogicAdapter is not implementing {nameof(IStateMachineDeviceBusinessLogicAdapter)}");
        }

        var configurator = stateMachineConfiguratorFactory.CreateInstance(adapter);
        configurator.ConfigureFactory();
        var stateFactory = configurator.BuildFactory();

        //// Important: store state factory instance to device and config
        //_orderManagementDevice.O = stateFactory;
    }
}