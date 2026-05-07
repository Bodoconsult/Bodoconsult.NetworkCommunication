// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="IpCommunicationHandler"/>
/// </summary>
public class IpCommunicationHandlerFactory : ICommunicationHandlerFactory
{
    private readonly ISocketProxyFactory _socketProxyFactory;
    private readonly IDuplexIoFactory _duplexIoFactory;
    //private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory;
    //private readonly ILogDataFactory _logDataFactory;
    //private readonly IAppLoggerProxyFactory _appLoggerFactory;
    private readonly IAppEventSourceFactory _appEventSourceFactory;
    private ICentralClientNotificationManager _clientNotificationManager;

    /// <summary>
    /// Default ctor for DI
    /// </summary>
    public IpCommunicationHandlerFactory(ISocketProxyFactory socketProxyFactory,
        IDuplexIoFactory duplexIoFactory,
        IAppEventSourceFactory appEventSourceFactory,
        ICentralClientNotificationManager clientNotificationManager)
    {
        _socketProxyFactory = socketProxyFactory;
        _duplexIoFactory = duplexIoFactory;
        _appEventSourceFactory = appEventSourceFactory;
        _clientNotificationManager = clientNotificationManager;
    }

    /// <summary>
    /// Create an instance implementing <see cref="ICommunicationHandler"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="ICommunicationHandler"/></returns>
    public ICommunicationHandler CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        var socketProxy = _socketProxyFactory.CreateInstance(dataMessagingConfig.IsServer, dataMessagingConfig.IpProtocol, IPAddress.Parse(dataMessagingConfig.IpAddress), dataMessagingConfig.Port, dataMessagingConfig.MonitorLogger);
        socketProxy.LoggerId = $"{dataMessagingConfig.LoggerId}{(dataMessagingConfig.LoggerId.EndsWith(": ") ? string.Empty: ": ")}";

        //if (dataMessagingConfig.MonitorLogger == null)
        //{
        //    var monitorLoggerFactory = _monitorLoggerFactoryFactory.CreateInstance($"{dataMessagingConfig.IpAddress}_{dataMessagingConfig.Port}");
        //    dataMessagingConfig.MonitorLogger = _appLoggerFactory.CreateInstance(monitorLoggerFactory, _logDataFactory);
        //}

        dataMessagingConfig.SocketProxy = socketProxy;

        var duplexIo = _duplexIoFactory.CreateInstance(dataMessagingConfig);

        return new IpCommunicationHandler(duplexIo, dataMessagingConfig, _appEventSourceFactory);
    }
}