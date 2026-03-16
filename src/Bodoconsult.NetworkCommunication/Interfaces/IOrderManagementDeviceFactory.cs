// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a device with order management
/// </summary>
public interface IOrderManagementDeviceFactory
{
    /// <summary>
    /// Create a device for order management only (no state management)
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    IOrderManagementDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig);
}