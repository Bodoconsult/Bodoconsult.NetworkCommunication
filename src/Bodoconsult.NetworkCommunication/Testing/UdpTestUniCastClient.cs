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
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    /// <param name="clientPort">Port the client listens on or 0 (then the same port as for the server is used). Setting clientPort is required normally only if UDP server and client are installed on the same machine!</param>
    public UdpTestUniCastClient(IPAddress ipAddress, int port, int clientPort=0) : base(ipAddress, port, clientPort)
    {
        var ep1 = new IPEndPoint(IPAddress.Any, ClientPort);
        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Bind(ep1);

        EndPoint = new IPEndPoint(ipAddress, ClientPort);
        SendEndPoint = new IPEndPoint(ipAddress, Port);
    }
}