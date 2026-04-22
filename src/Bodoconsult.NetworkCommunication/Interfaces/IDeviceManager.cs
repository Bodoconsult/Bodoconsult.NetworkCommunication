// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Basic interface for device manager
/// </summary>
public interface IDeviceManager
{
    /// <summary>
    /// Current device
    /// </summary>
    IIpDevice? IpDevice { get; }
}