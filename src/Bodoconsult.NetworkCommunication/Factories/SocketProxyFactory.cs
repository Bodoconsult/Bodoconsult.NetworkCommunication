// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory to create an instance of <see cref="TcpIpServerSocketProxy"/>, <see cref="UdpServerSocketProxy"/>, <see cref="TcpIpClientSocketProxy"/> or <see cref="UdpClientSocketProxy"/>
/// </summary>
public class SocketProxyFactory : ISocketProxyFactory
{
    private readonly ITcpIpListenerManager? _tcpIpListenerManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="tcpIpListenerManager">Current listener manager</param>
    public SocketProxyFactory(ITcpIpListenerManager? tcpIpListenerManager)
    {
        _tcpIpListenerManager = tcpIpListenerManager;
    }

    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <param name="isServer">Is the device configured as IP server. True = server, false = client. Default: false</param>
    /// <param name="protocol">IP base protocol to be used</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    public ISocketProxy CreateInstance(bool isServer, IpProtocolEnum protocol, IPAddress ipAddress, int port)
    {
        if (isServer)
        {
            if (protocol == IpProtocolEnum.Udp)
            {
                return new UdpServerSocketProxy
                {
                    IpAddress = ipAddress,
                    Port = port,
                };
            }

            ArgumentNullException.ThrowIfNull(_tcpIpListenerManager);

            return new TcpIpServerSocketProxy(_tcpIpListenerManager)
            {
                IpAddress = ipAddress,
                Port = port
            };
        }


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