// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpDevice.Bll.Interfaces;

/// <summary>
/// IP config of a device
/// </summary>
public struct IpConfig
{
    /// <summary>
    /// Current IP address
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Current port
    /// </summary>
    public int Port { get; set; }
}