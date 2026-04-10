// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

// Taken from https://sashadu.wordpress.com/2016/06/10/c-udp-multicast/

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

/// <summary>
/// Multicast UdpClient wrapper with send and receive capabilities.
/// Usage: pass local and remote multicast IPs and port to constructor.
/// Use Send method to send data,
/// subscribe to Received event to get notified about received data.
/// </summary>
public class MulticastUdpClient: IDisposable
{
    private readonly UdpClient Listener;
    private int _port;
    private readonly IPEndPoint EndPoint;

    public MulticastUdpClient(IPAddress multicastIPaddress, int port)
    {
        _port = port;
        var localIPaddress1 = IPAddress.Any;
        
        // Create endpoints
        EndPoint = new IPEndPoint(multicastIPaddress, port);
        var localEndPoint = new IPEndPoint(localIPaddress1!, port);

        // Create and configure UdpClient
        Listener = new UdpClient();
        // The following three lines allow multiple clients on the same PC
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.ExclusiveAddressUse = false;
        // Bind, Join
        Listener.Client.Bind(localEndPoint);
        Listener.JoinMulticastGroup(multicastIPaddress, localIPaddress1!);
    }

    /// <summary>
    /// Send the buffer by UDP to multicast address
    /// </summary>
    /// <param name="bufferToSend"></param>
    public void Send(byte[] bufferToSend)
    {
        Listener.Send(bufferToSend, bufferToSend.Length, EndPoint);
    }

    /// <summary>
    /// Receive data
    /// </summary>
    /// <returns>Received data</returns>
    public byte[] Receive()
    {
        var sender = new IPEndPoint(0, 0);
        var bytes = Listener.Receive(ref sender);

        // No data received?
        if (bytes.Length == 0)
        {
            return bytes;
        }

        Debug.Print($"received {bytes.Length} bytes from {sender}");
        return bytes;
    }

    public void Dispose()
    {
        Listener.Dispose();
    }
}