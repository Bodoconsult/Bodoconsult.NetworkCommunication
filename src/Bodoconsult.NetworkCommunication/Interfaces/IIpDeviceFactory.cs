// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a device without order management and state management
/// </summary>
public interface IIpDeviceFactory
{
    /// <summary>
    /// Create a device for order management only (no state management)
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    IIpDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig);
}