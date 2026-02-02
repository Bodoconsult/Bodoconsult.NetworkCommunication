// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple multicast UDP server for testing purposes
/// </summary>
public class UdpTestMultiCastServer : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public UdpTestMultiCastServer(IPAddress ipAddress, int port) : base(new IPEndPoint(ipAddress, port))
    {
        IsServer = true;

        var endPoint1 = new IPEndPoint(0, EndPoint.Port);
        Listener.JoinMulticastGroup(ipAddress, 50);
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);
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