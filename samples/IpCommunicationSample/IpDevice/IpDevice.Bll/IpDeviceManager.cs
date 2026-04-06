// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpDevice.Bll.BusinessTransactions.Providers;
using IpDevice.Bll.Communication;
using IpDevice.Bll.Interfaces;
using Newtonsoft.Json.Linq;

namespace IpDevice.Bll;

/// <summary>
/// Top level class to handle the comm for the IP device
/// </summary>
public class IpDeviceManager : IIpDeviceManager
{
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    private readonly ILogDataFactory _logDataFactory;
    private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppLoggerProxy _appLogger;
    private readonly ISendPacketProcessFactory _sendPacketProcessFactory;
    private readonly ITcpIpListenerManager _tcpIpListenerManager;
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logDataFactory">Current log data factory</param>
    /// <param name="appLoggerFactory">Current logger proxy factory</param>
    /// <param name="appEventSourceFactory">Current factory for <see cref="IAppEventSource"/> instances</param>
    /// <param name="clientNotificationManager">Current client notification manager instance</param>
    /// <param name="monitorLoggerFactoryFactory">Current factory for monitor logger factories</param>
    /// <param name="appLogger">Current app logger</param>
    /// <param name="sendPacketProcessFactory">Current send packet process factory</param>
    /// <param name="tcpIpListenerManager">Current TCP/IP listener manager</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public IpDeviceManager(IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory,
        ILogDataFactory logDataFactory,
        IAppLoggerProxyFactory appLoggerFactory,
        IAppEventSourceFactory appEventSourceFactory,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppLoggerProxy appLogger,
        ISendPacketProcessFactory sendPacketProcessFactory,
        ITcpIpListenerManager tcpIpListenerManager,
        IBusinessTransactionManager businessTransactionManager
        )
    {
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
        _monitorLoggerFactoryFactory = monitorLoggerFactoryFactory;
        _appLoggerFactory = appLoggerFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _logDataFactory = logDataFactory;
        _appLogger = appLogger;
        _sendPacketProcessFactory = sendPacketProcessFactory;
        _tcpIpListenerManager = tcpIpListenerManager;
        _businessTransactionManager = businessTransactionManager;
    }

    /// <summary>
    /// Current IP config of the backend for TCP/IP
    /// </summary>
    public IpConfig? BackendTcpIpConfig { get; set; }

    /// <summary>
    /// Current IP config of the backend for UDP
    /// </summary>
    public IpConfig? BackendUdpConfig { get; set; }

    /// <summary>
    /// Represents the TCP/IP communication with the backend
    /// </summary>
    public ISimpleDeviceManager? BackendTcpIp { get; private set; }

    /// <summary>
    /// Represents the UDP communication with the backend
    /// </summary>
    public ISimpleDeviceManager? BackendUdp { get; private set; }

    /// <summary>
    /// Load the comm via TCP/IP to the backend
    /// </summary>
    public void LoadBackendTcpIp()
    {
        if (!BackendTcpIpConfig.HasValue)
        {
            throw new ArgumentNullException(nameof(BackendTcpIpConfig));
        }

        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var m = new BackendTcpIpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger, _businessTransactionManager);

        // Act  
        m.ConfigureDevice(BackendTcpIpConfig.Value.IpAddress, BackendTcpIpConfig.Value.Port);

        BackendTcpIp = m;
    }

    /// <summary>
    /// Load the comm via UDP to the dbackend
    /// </summary>
    public void LoadBackendUdp()
    {
        if (!BackendUdpConfig.HasValue)
        {
            throw new ArgumentNullException(nameof(BackendUdpConfig));
        }

        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var m = new BackendUdpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger);

        // Act  
        m.ConfigureDevice(BackendUdpConfig.Value.IpAddress, BackendUdpConfig.Value.Port);

        BackendUdp = m;
    }

    /// <summary>
    /// Load the business transactions required for the app
    /// </summary>
    public void LoadBusinessTransactions()
    {
        ArgumentNullException.ThrowIfNull(BackendUdp);
        var adapter = (IBackendUdpBusinessLogicAdapter?)BackendUdp.DeviceBusinessLogicAdapter;

        ArgumentNullException.ThrowIfNull(adapter);

        var provider = new BackendUdpBusinessTransactionProvider(adapter);
        _businessTransactionManager.AddProvider(provider);
    }
}