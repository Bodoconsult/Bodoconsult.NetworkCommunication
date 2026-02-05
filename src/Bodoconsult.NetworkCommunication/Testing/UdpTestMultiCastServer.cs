// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple multicast UDP server for testing purposes
/// </summary>
public class UdpTestMultiCastServer : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server is listening on</param>
    /// <param name="clientPort">Port the client listens on or 0 (then the same port as for the server is used). Setting clientPort is required normally only if UDP server and client are installed on the same machine!</param>
    public UdpTestMultiCastServer(IPAddress ipAddress, int port, int clientPort = 0) : base(ipAddress, port, clientPort)
    {
        IsServer = true;

        var endPoint1 = new IPEndPoint(0, Port);
        Listener.JoinMulticastGroup(ipAddress, 50);
        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);

        EndPoint = new IPEndPoint(ipAddress, Port);
        SendEndPoint = new IPEndPoint(ipAddress, ClientPort);
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