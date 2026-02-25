// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="TcpIpServerSocketProxy"/> or <see cref="UdpServerSocketProxy"/>
/// </summary>
public class ServerSocketProxyFactory : ISocketProxyFactory
{
    private readonly ITcpIpListenerManager _tcpIpListenerManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="tcpIpListenerManager">Current listener manager</param>
    public ServerSocketProxyFactory(ITcpIpListenerManager tcpIpListenerManager)
    {
        _tcpIpListenerManager = tcpIpListenerManager;
    }

    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <param name="protocol">IP base protocol to be used</param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance(IpProtocolEnum protocol)
    {
        if (protocol == IpProtocolEnum.Udp)
        {
            return new UdpServerSocketProxy();
        }

        return new TcpIpServerSocketProxy(_tcpIpListenerManager);
    }
}