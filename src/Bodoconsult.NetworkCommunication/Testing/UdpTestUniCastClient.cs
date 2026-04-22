// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// A simple UDP unicast client for testing
/// </summary>
public class UdpTestUniCastClient: BaseUdpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">Server IP address</param>
    /// <param name="port">Port the server is listening on</param>
    public UdpTestUniCastClient(IPAddress ipAddress, int port) : base(ipAddress, port, false, false)
    {
        // The following three lines allow multiple clients on the same PC
        //Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.Blocking = false;

        EndPoint = new IPEndPoint(ipAddress, Port);
        Listener.Connect(EndPoint);
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        if (IsDisposed)
        {
            return;
        }

        var result = Listener.Send(data, data.Length);
        Trace.TraceInformation($"{TypeName}: sent {result} byte(s)!");
    }
}