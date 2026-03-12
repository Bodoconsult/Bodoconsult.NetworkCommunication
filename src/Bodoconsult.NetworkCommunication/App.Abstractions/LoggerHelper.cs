// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Helper class for logging
/// </summary>
public static class LoggerHelper
{
    /// <summary>
    /// Current fake logger
    /// </summary>
    public static IAppLoggerProxy FakeLogger { get; } = new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory());
}