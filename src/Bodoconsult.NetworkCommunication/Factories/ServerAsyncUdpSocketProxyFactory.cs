// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create a server instance of <see cref="AsyncUdpSocketProxy"/>
/// </summary>
public class ServerAsyncUdpSocketProxyFactory : ISocketProxyFactory
{
    /// <summary>
    /// Creates a server instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance()
    {
        return new AsyncUdpSocketProxy(true);
    }
}