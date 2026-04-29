// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

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
    public UdpTestMultiCastClient(IPAddress ipAddress, int port) : base(ipAddress, port, false, true)
    {
        var localIPaddress1 = IPAddress.Any;

        // Create endpoints
        EndPoint = new IPEndPoint(ipAddress, port);
        var localEndPoint = new IPEndPoint(localIPaddress1!, port);

        // The following three lines allow multiple clients on the same PC
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        // Bind, Join
        Listener.Client.Bind(localEndPoint);
        Listener.JoinMulticastGroup(ipAddress, localIPaddress1!);
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        var result = Listener.Send(data, data.Length, EndPoint);
        Trace.TraceInformation($"{TypeName}: sent {result} byte(s)!");
    }

    /// <summary>
    /// Receive data
    /// </summary>
    /// <returns>Received data</returns>
    public override async Task<byte[]> Receive()
    {
        if (IsDisposed)
        {
            Trace.TraceInformation($"{TypeName}: nothing to receive");
            return [];
        }

        var bytes = await Listener.ReceiveAsync();

        // No data received?
        if (bytes.Buffer.Length == 0)
        {
            return [];
        }

        Trace.TraceInformation($"{TypeName}: received {bytes.Buffer.Length} bytes from {bytes.RemoteEndPoint}");
        //Trace.TraceInformation($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

        ReceivedMessages.Add(bytes.Buffer.AsMemory());
        return bytes.Buffer;
    }
}