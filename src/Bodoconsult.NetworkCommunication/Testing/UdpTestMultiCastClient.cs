// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UPD multicast client for testing
/// </summary>
public class UdpTestMultiCastClient : BaseUdpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server is listening on</param>
    public UdpTestMultiCastClient(IPAddress ipAddress, int port) : base(ipAddress, port, false)
    {
        var endPoint1 = new IPEndPoint(0, Port);
        Listener.Client.Bind(endPoint1);

        Listener.JoinMulticastGroup(ipAddress);
        Listener.MulticastLoopback = true;

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