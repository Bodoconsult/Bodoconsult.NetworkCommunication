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
    public string IpAddress { get; set; }

    /// <summary>
    /// Port to use for communication
    /// </summary>
    public int Port { get; set; }
}