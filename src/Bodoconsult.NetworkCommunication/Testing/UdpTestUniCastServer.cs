// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple unicast UDP server for testing purposes
/// </summary>
public class UdpTestUniCastServer : UdpBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    /// <param name="clientPort">Port the client listens on or 0 (then the same port as for the server is used)</param>
    public UdpTestUniCastServer(IPAddress ipAddress, int port, int clientPort = 0) : base(ipAddress, port, clientPort)
    {
        IsServer = true;

        var endPoint1 = new IPEndPoint(IPAddress.Any, Port);
        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(endPoint1);

        EndPoint = new IPEndPoint(ipAddress, Port);
        SendEndPoint = new IPEndPoint(ipAddress, ClientPort);
    }
}

