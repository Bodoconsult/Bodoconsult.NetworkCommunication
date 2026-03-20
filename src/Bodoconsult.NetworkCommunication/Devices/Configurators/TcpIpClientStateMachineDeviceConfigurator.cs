// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;

namespace Bodoconsult.NetworkCommunication.Devices.Configurators;

/// <summary>
/// Configurator for the communication to the IP device via TCP/IP (client side) with state management
/// </summary>
public class TcpIpClientStateMachineDeviceConfigurator : BaseIpDeviceConfigurator
{
    private readonly IDuplexIoFactory _duplexIoFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLoggerProxy;

    private IStateMachineDevice? _stateManagementDevice;

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
    public TcpIpClientStateMachineDeviceConfigurator(IDuplexIoFactory duplexIoFactory,
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
        DataMessagingConfig.StateMachineProcessingPackage = new StateMachineProcessingPackage()
        {
            StateCheckManager = new DoNothingStateCheckManager()
        };
    }

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    public override void CreateDevice()
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        // Server
        var socketProxyFactory = new SocketProxyFactory(null);

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, _duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new BasicManagementDeviceFactory(_clientNotificationManager, commAdapterFactory);
        Device = factory.CreateInstance(DataMessagingConfig);

        if (Device is not IStateMachineDevice smd)
        {
            throw new ArgumentException($"Device must implement {nameof(IStateMachineDevice)}");
        }

        _stateManagementDevice = smd;
    }

    /// <summary>
    /// Configure the order management
    /// </summary>
    /// <param name="orderManagerFactory">Current factory for <see cref="IOrderManager"/> instances</param>
    public override void ConfigureOrderManagement(IOrderManagerFactory orderManagerFactory)
    {
        ArgumentNullException.ThrowIfNull(_stateManagementDevice);

        // Order management
        var om = orderManagerFactory.CreateInstance(_stateManagementDevice);
        _stateManagementDevice.LoadDeviceOrderManager(om);
    }


    /// <summary>
    /// Configure the order management and if necessary the state management. Important: store state factory instance to device and config
    /// </summary>
    /// <param name="deviceBusinessLogicAdapterFactory">Current factory for <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instances</param>
    /// <param name="stateMachineConfiguratorFactory">Current state machine configurator factory</param>
    public override void ConfigureStateManagement(IDeviceBusinessLogicAdapterFactory deviceBusinessLogicAdapterFactory,
        IStateMachineConfiguratorFactory stateMachineConfiguratorFactory)
    {
        ArgumentNullException.ThrowIfNull(_stateManagementDevice);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig?.StateMachineProcessingPackage, "DataMessagingConfig or StateMachineProcessingPackage is null");

        var dsm = deviceBusinessLogicAdapterFactory.CreateInstance(_stateManagementDevice);

        if (dsm is not IStateMachineDeviceBusinessLogicAdapter adapter)
        {
            throw new ArgumentException("dsm is not implementing " + nameof(IStateMachineDeviceBusinessLogicAdapter));
        }

        _stateManagementDevice.LoadDeviceBusinessLogicAdapter(adapter);

        var configurator = stateMachineConfiguratorFactory.CreateInstance(adapter);
        configurator.ConfigureFactory();
        var stateFactory = configurator.BuildFactory();

        // Important: store state factory instance to device and config
        _stateManagementDevice.StateMachineStateFactory = stateFactory;
        DataMessagingConfig.StateMachineProcessingPackage.StateMachineStateFactory = stateFactory;
    }
}