// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UPD unicast client for testing
/// </summary>
public class UdpTestUniCastClient: UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public UdpTestUniCastClient(IPAddress ipAddress, int port) : base(new IPEndPoint(ipAddress, port))
    {
        var endPoint1 = new IPEndPoint(0, EndPoint.Port);
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);
        Listener.Connect(EndPoint);
    }
}