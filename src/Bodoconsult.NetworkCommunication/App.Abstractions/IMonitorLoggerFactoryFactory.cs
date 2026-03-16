// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Interface to create <see cref="IMonitorLoggerFactory"/> factories
/// </summary>
public interface IMonitorLoggerFactoryFactory
{
    /// <summary>
    /// Create a monitor logger factory
    /// </summary>
    /// <param name="deviceName">Current tower serial number</param>
    /// <returns></returns>
    IMonitorLoggerFactory CreateInstance(string deviceName);
}

