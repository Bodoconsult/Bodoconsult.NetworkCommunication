// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="TcpIpClientSocketProxy"/> or <see cref="UdpClientSocketProxy"/>
/// </summary>
public class ClientSocketProxyFactory : ISocketProxyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <param name="protocol">IP base protocol to be used</param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance(IpProtocolEnum protocol)
    {
        if (protocol == IpProtocolEnum.Udp)
        {
            return new UdpClientSocketProxy();
        }
        return new TcpIpClientSocketProxy();
    }
}