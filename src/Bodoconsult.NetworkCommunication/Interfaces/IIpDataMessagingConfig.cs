// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Configuration to use for IP based data messaging (UDP / TCP)
/// </summary>
public interface IIpDataMessagingConfig: IDataMessagingConfig
{
    /// <summary>
    /// IP address
    /// </summary>
    string IpAddress { get; set; }

    /// <summary>
    /// Port to use for communication
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// Is the device configured as IP server. True = server, false = client
    /// </summary>
    bool IsServer { get; set; }
}