// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Logging;

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
}