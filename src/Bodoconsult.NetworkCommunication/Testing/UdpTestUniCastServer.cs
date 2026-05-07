// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple unicast UDP server for testing purposes: Sending only!
/// </summary>
public class UdpTestUniCastServer : BaseUdpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    public UdpTestUniCastServer(IPAddress ipAddress, int port) : base(ipAddress, port, true, false)
    {
        //var endPoint1 = new IPEndPoint(IPAddress.Any, Port);
        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //Listener.Client.Bind(endPoint1);

        // The following three lines allow multiple clients on the same PC
        //Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Blocking = false;

        EndPoint = new IPEndPoint(ipAddress, Port);
        SenderEndPoint = EndPoint;

        //// Remove
        //SendEndPoint = new IPEndPoint(ipAddress, RemotePort);
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        if (IsDisposed || SenderEndPoint == null)
        {
            return;
        }

        var result = Listener.Send(data, data.Length, SenderEndPoint);
        Debug.Print($"{TypeName}: sent {result} byte(s)!");
    }
}

