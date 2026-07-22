// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataExportServices;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
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
    private readonly IBusinessTransactionManager _businessTransactionManager;

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
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public IpDeviceUdpClientManager(IDuplexIoFactory duplexIoFactory,
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLoggerProxy,
        ISocketProxyFactory socketProxyFactory,
        IAppGlobals appGlobals,
        IBusinessTransactionManager businessTransactionManager)
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
        _businessTransactionManager = businessTransactionManager;
    }

    /// <summary>
    /// Current <see cref="ISimpleDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public ISimpleDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; private set; }

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

        // Create config
        configurator.CreateMessagingConfig("Backend_Device_UDP: ", ipAddress, port);

        var config = configurator.DataMessagingConfig;

        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(configurator.DataMessagingConfig);

        // No handshakes
        config.AnswerWithAcknowledgement = false;
        config.WaitForAcknowledgement = false;

        config.BufferSize = 16384;
        configurator.DataMessagingConfig.ExpectedMaximumLength = 16384;

        // Create data loggers
        var appStartParams = (BackendAppStartParameter)_appGlobals.AppStartParameter;
        config.DataLoggingPath = appStartParams.DataLoggingPath;

        _appGlobals.Logger?.LogInformation($"Data logging to folder {appStartParams.DataLoggingPath}");

        var fileSize = appStartParams.MaxDataLoggingFileSize;

        const int cacheSize = 5;

        CreateLoggerChannel(0x0, ipAddress, config, "Channel1", fileSize, cacheSize, true);
        CreateLoggerChannel(0x1, ipAddress, config, "Channel2", fileSize, cacheSize, true);
        CreateLoggerChannel(0x2, ipAddress, config, "Channel3", fileSize, cacheSize, true);
        CreateLoggerChannel(0x3, ipAddress, config, "Channel4", fileSize, cacheSize, true);
        CreateLoggerChannel(0xC, ipAddress, config, "ADD", fileSize, 5, false);

        // Create messaging package
        configurator.CreateDataMessagingPackage(messageProcessingPackageFactory);

        // Create the device now
        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new SfxpIpDeviceUdpBusinessLogicAdapterFactory(_businessTransactionManager);
        configurator.CreateDevice(businessLogicAdapterFactory);

        var device = configurator.GetDevice();

        if (device.DeviceBusinessLogicAdapter is not ISimpleDeviceBusinessLogicAdapter dbla)
        {
            throw new ArgumentException($"device.DeviceBusinessLogicAdapter does not implement {nameof(ISimpleDeviceBusinessLogicAdapter)}");
        }

        IpDevice = device;
        DeviceBusinessLogicAdapter = dbla;
    }

    private static void CreateLoggerChannel(byte channel, string ipAddress, IDataMessagingConfig config, string channelName, long size, int cacheSize, bool isHigherPriority)
    {
        config.AppLogger.LogInformation($"{config.LoggerId}: storing bin data for {channelName} to {config.DataLoggingPath}");

        config.DataLoggingFileName = $"IPDevice_{ipAddress.Replace(".", "_", StringComparison.InvariantCultureIgnoreCase)}_{channelName}";
        config.DataLoggingPath = config.DataLoggingPath;

        var es = new ByteArrayDataExportService
        {
            FileName = config.DataLoggingFileName,
            TargetPath = config.DataLoggingPath,
            CacheSize = cacheSize,
            FileExtension = "bin",
            MaxFileSize = size,
            FlushInterval = 3
        };

        if (isHigherPriority)
        {
            es.ThreadPriority = ThreadPriority.AboveNormal;
        }

        var logger = new SfxpDataChunkInboundDataLogger(es)
        {
            Channel = channel
        };
        config.DataLoggers.Add(logger);
    }
}