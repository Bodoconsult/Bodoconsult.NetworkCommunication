// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="AsyncTcpIpSocketProxy"/>
/// </summary>
public class AsyncTcpIpSocketProxyFactory : ISocketProxyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance()
    {
        return new AsyncTcpIpSocketProxy();
    }
}