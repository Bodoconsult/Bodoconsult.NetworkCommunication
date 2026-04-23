// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
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
    private readonly ISocketProxyFactory _socketProxyFactory;

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
    /// <param name="socketProxyFactory">Current socket factory</param>
    public TcpIpClientStateMachineDeviceConfigurator(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy,
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
        _socketProxyFactory = socketProxyFactory;
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
        DataMessagingConfig.LoggerId = loggerId;
        DataMessagingConfig.AppLogger = _appLoggerProxy;
        //DataMessagingConfig.AppLogger = new AppLoggerProxy(new FakeLoggerFactory(), _logDataFactory);
        DataMessagingConfig.MonitorLogger = CreateMonitorLogger(_monitorLoggerFactoryFactory, _appLoggerFactory, _logDataFactory, DataMessagingConfig.LoggerId.Replace(" ", "").Replace(":", ""));
        //DataMessagingConfig.MonitorLogger = new AppLoggerProxy(new FakeLoggerFactory(), _logDataFactory);
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
        var communicationHandlerFactory = new IpCommunicationHandlerFactory(_socketProxyFactory, _duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory);

        var factory = new BasicStateMachineDeviceFactory(_clientNotificationManager, commAdapterFactory);
        IDeviceStateCheckManager deviceStateCheckManager = new DoNothingStateCheckManager();
        Device = factory.CreateInstance(DataMessagingConfig, deviceStateCheckManager);

        if (Device is not IStateMachineDevice smd)
        {
            throw new ArgumentException($"Device must implement {nameof(IStateMachineDevice)}");
        }

        var dsm = businessLogicAdapterFactory.CreateInstance(Device);

        if (dsm is not IStateMachineDeviceBusinessLogicAdapter adapter)
        {
            throw new ArgumentException($"dsm is not implementing {nameof(IStateMachineDeviceBusinessLogicAdapter)}");
        }

        Device.LoadDeviceBusinessLogicAdapter(adapter);

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
    /// <param name="stateMachineConfiguratorFactory">Current state machine configurator factory</param>
    public override void ConfigureStateManagement(IStateMachineConfiguratorFactory stateMachineConfiguratorFactory)
    {
        ArgumentNullException.ThrowIfNull(Device);
        ArgumentNullException.ThrowIfNull(_stateManagementDevice);

        if (Device.DeviceBusinessLogicAdapter is not IStateMachineDeviceBusinessLogicAdapter adapter)
        {
            throw new ArgumentException($"Device.DeviceBusinessLogicAdapter is not implementing {nameof(IStateMachineDeviceBusinessLogicAdapter)}");
        }

        var configurator = stateMachineConfiguratorFactory.CreateInstance(adapter);
        configurator.ConfigureFactory();
        var stateFactory = configurator.BuildFactory();

        // Important: store state factory instance to device and config
        _stateManagementDevice.StateMachineStateFactory = stateFactory;
        adapter.LoadStateFactory(stateFactory);
        adapter.LoadOrderFactory();
    }
}