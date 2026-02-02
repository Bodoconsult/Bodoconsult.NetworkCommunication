// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;


namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// High performance pipeline by MS based implementation of <see cref="IDuplexIo"/>
/// </summary>
public class IpHighPerformanceDuplexIoFactory : IDuplexIoFactory
{

    private readonly ISendPacketProcessFactory _sendPacketProcessFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpHighPerformanceDuplexIoFactory(ISendPacketProcessFactory sendPacketProcessFactory)
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
        return new IpHighPerformanceDuplexIo(config, _sendPacketProcessFactory);
    }
}