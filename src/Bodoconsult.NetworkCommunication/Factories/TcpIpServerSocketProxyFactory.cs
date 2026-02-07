// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="TcpIpServerSocketProxy"/>
/// </summary>
public class TcpIpServerSocketProxyFactory : ISocketProxyFactory
{
    private readonly ITcpIpListenerManager _tcpIpListenerManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="tcpIpListenerManager">Current listener manager</param>
    public TcpIpServerSocketProxyFactory(ITcpIpListenerManager tcpIpListenerManager)
    {
        _tcpIpListenerManager = tcpIpListenerManager;
    }
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance()
    {
        return new TcpIpServerSocketProxy(_tcpIpListenerManager);
    }
}