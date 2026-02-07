// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UPD unicast client for testing
/// </summary>
public class UdpTestUniCastClient: UdpDeviceBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    /// <param name="remotePort">Port the remote device listens on or 0 (then the same port as for the current device is used). Setting remotePort is required normally only if UDP server and client are installed on the same machine!</param>
    public UdpTestUniCastClient(IPAddress ipAddress, int port, int remotePort=0) : base(ipAddress, port, remotePort)
    {
        var ep1 = new IPEndPoint(IPAddress.Any, RemotePort);
        Listener.Client.Bind(ep1);

        EndPoint = new IPEndPoint(ipAddress, RemotePort);
        SendEndPoint = new IPEndPoint(ipAddress, Port);
    }
}