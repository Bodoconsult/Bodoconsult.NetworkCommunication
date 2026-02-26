// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class TestDataHelper
{

    static TestDataHelper()
    {
        LogDataFactory = new LogDataFactory();
        LoggingConfig = new LoggingConfig();
    }

    public static LogDataFactory LogDataFactory { get; }

    public static LoggingConfig LoggingConfig { get; }

    /// <summary>
    /// Get a full set up fake logger
    /// </summary>
    /// <returns>Logger instance</returns>
    public static AppLoggerProxy GetFakeAppLoggerProxy()
    {
        if (_logger != null)
        {
            return _logger;
        }
        _logger = new AppLoggerProxy(new FakeLoggerFactory(), LogDataFactory);
        return _logger;
    }

    private static AppLoggerProxy _logger;

    /// <summary>
    /// Get a full set up fake bench logger
    /// </summary>
    /// <returns>Bench logger instance</returns>
    public static AppBenchProxy GetFakeAppBenchProxy()
    {
        if (_bench != null)
        {
            return _bench;
        }
        _bench = new AppBenchProxy(new FakeLoggerFactory(), LogDataFactory);
        return _bench;
    }

    private static AppBenchProxy _bench;

    /// <summary>
    ///  Get a random port (range should be configured in Wiondws Firewall for TCP and UDP). Remember: for UDP is a second port number calculated as port number 2 = port number 1 plus 1 required
    /// </summary>
    /// <returns></returns>
    public static int GetRandomPort()
    {
        return new Random(33025).Next(33000, 33049);
    }

    /// <summary>
    /// Get a messaging config
    /// </summary>
    /// <returns>Data messaging config</returns>
    public static IIpDataMessagingConfig GetDataMessagingConfig()
    {
        var config = new DefaultDataMessagingConfig();

        config.DataMessageProcessingPackage = new BtcpDataMessageProcessingPackage(config);

        return config;
    }

    /// <summary>
    /// Get a fake <see cref="IAppDateService"/> instance
    /// </summary>
    /// <returns></returns>
    public static IAppDateService GetFakeDateTimeService()
    {
        // ToDo: change to fake later
        return new AppDateService();
    }
}