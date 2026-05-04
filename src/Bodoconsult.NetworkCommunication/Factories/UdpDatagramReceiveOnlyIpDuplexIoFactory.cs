// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for <see cref="UdpDatagramReceiveOnlyIpDuplexIo"/> instances
/// </summary>
public class UdpDatagramReceiveOnlyIpDuplexIoFactory : IDuplexIoFactory
{
    private readonly ISendPacketProcessFactory _sendPacketProcessFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    public UdpDatagramReceiveOnlyIpDuplexIoFactory(ISendPacketProcessFactory sendPacketProcessFactory)
    {
        _sendPacketProcessFactory = sendPacketProcessFactory;
    }

    /// <summary>
    /// Creates an instance of <see cref="IDuplexIo"/>
    /// </summary>
    /// <param name="config">Current data messaging config</param>
    /// <returns>Instance of <see cref="IDuplexIo"/></returns>
    public IDuplexIo CreateInstance(IDataMessagingConfig config)
    {
        return new UdpDatagramReceiveOnlyIpDuplexIo(config, _sendPacketProcessFactory);
    }
}