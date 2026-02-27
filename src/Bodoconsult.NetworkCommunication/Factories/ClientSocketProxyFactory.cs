// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using System.Net;

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
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance(IpProtocolEnum protocol, IPAddress ipAddress, int port)
    {
        if (protocol == IpProtocolEnum.Udp)
        {
            return new UdpClientSocketProxy
                {
                    IpAddress = ipAddress,
                    Port = port
                };
        }
        return new TcpIpClientSocketProxy
            {
                IpAddress = ipAddress,
                Port = port
            };
    }
}