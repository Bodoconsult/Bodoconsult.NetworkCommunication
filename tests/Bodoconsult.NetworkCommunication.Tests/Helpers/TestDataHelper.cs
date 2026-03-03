// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class TestDataHelper
{
    static TestDataHelper()
    {
        LogDataFactory = new LogDataFactory();
        LoggingConfig = new LoggingConfig();
        AppEventSourceFactory = new FakeAppEventSourceFactory();
        // ToDo: change to fake later
        AppDateService = new AppDateService();
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
    /// Get a fake <see cref="NetworkCommunication.App.Abstractions.IAppDateService"/> instance
    /// </summary>
    public static IAppDateService AppDateService { get; }

    /// <summary>
    /// Get a fake <see cref="IAppEventSourceFactory"/> instance
    /// </summary>
    public static IAppEventSourceFactory AppEventSourceFactory { get; }

    /// <summary>
    /// Create a SDCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = new SdcpOrder(ps, AppDateService, GetFakeAppBenchProxy());
        return order;
    }
}