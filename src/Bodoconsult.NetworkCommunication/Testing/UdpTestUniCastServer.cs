// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple unicast UDP server for testing purposes
/// </summary>
public class UdpTestUniCastServer : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public UdpTestUniCastServer(IPAddress ipAddress, int port) : base(new IPEndPoint(ipAddress, port))
    {
        IsServer = true;

        var endPoint1 = new IPEndPoint(0, EndPoint.Port);
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);
        Listener.Connect(EndPoint);
    }
}

