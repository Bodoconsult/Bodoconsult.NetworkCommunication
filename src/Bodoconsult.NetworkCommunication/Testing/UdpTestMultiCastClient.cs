// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UPD multicast client for testing
/// </summary>
public class UdpTestMultiCastClient : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public UdpTestMultiCastClient(IPAddress ipAddress, int port) : base(new IPEndPoint(ipAddress, port))
    {
        var endPoint1 = new IPEndPoint(0, EndPoint.Port);
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);

        Listener.JoinMulticastGroup(ipAddress);
        Listener.MulticastLoopback = true;
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        var result = Listener.Send(data, EndPoint);
        Debug.Print($"{GetType().Name}: sent {result} byte(s)!");
    }
}