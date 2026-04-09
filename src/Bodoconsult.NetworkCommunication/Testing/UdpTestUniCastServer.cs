// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple unicast UDP server for testing purposes
/// </summary>
public class UdpTestUniCastServer : BaseUdpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    public UdpTestUniCastServer(IPAddress ipAddress, int port) : base(ipAddress, port, true)
    {
        //var endPoint1 = new IPEndPoint(IPAddress.Any, Port);
        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //Listener.Client.Bind(endPoint1);

        ReceiceEndPoint = new IPEndPoint(IPAddress.Any, Port);

        //// Remove
        //SendEndPoint = new IPEndPoint(ipAddress, RemotePort);
    }

    public override void Send(byte[] data)
    {
        if (IsDisposed)
        {
            return;
        }

        var result = Listener.Send(data, data.Length, ReceiceEndPoint);
        Debug.Print($"{TypeName}: sent {result} byte(s)!");
    }
}

