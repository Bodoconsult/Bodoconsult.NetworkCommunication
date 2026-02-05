// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UPD multicast client for testing
/// </summary>
public class UdpTestMultiCastClient : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server is listening on</param>
    /// <param name="clientPort">Port the client listens on or 0 (then the same port as for the server is used)</param>
    public UdpTestMultiCastClient(IPAddress ipAddress, int port, int clientPort = 0) : base(ipAddress, port, clientPort)
    {
        var endPoint1 = new IPEndPoint(0, Port);
        Listener.Client.Bind(endPoint1);


        Listener.JoinMulticastGroup(ipAddress);
        Listener.MulticastLoopback = true;

        EndPoint = new IPEndPoint(ipAddress, ClientPort);
        SendEndPoint = new IPEndPoint(ipAddress, Port);
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