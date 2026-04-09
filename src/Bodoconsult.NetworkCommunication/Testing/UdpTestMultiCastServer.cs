// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple multicast UDP server for testing purposes
/// </summary>
public class UdpTestMultiCastServer : BaseUdpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server is listening on</param>
    public UdpTestMultiCastServer(IPAddress ipAddress, int port) : base(ipAddress, port, true)
    {
        var endPoint1 = new IPEndPoint(0, Port);
        Listener.JoinMulticastGroup(ipAddress, 50);
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);

        ReceiceEndPoint = new IPEndPoint(ipAddress, Port);
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        var result = Listener.Send(data, ReceiceEndPoint);
        Debug.Print($"{GetType().Name}: sent {result} byte(s)!");
    }
}