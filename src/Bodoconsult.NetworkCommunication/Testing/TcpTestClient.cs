// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpTestClient :   BaseTcpIpDevice
{
    private readonly IPEndPoint _endPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Local port</param>
    public TcpTestClient(IPAddress ipAddress, int port)
    {
        IsServer = false;

        Debug.Print($"Client: port {port}");

        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout
        };
        Socket.ExclusiveAddressUse = false;
        Socket.NoDelay = true;
        Socket.Blocking = false;

        _endPoint = new IPEndPoint(ipAddress, port);
        Socket.ConnectAsync(_endPoint).Wait(5000);
    }

    /// <summary>
    /// Reset the client socket if necessary
    /// </summary>
    public override void ResetClientSocket()
    {
        //if (_clientSocket == null)
        //{
        //    return;
        //}
        //_clientSocket.Close();
        //_clientSocket = null;
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(Socket);

        var task = Socket.SendToAsync(data, _endPoint);
        task.Wait(CancellationTokenSource.Token);

        Debug.Print($"TcpClient: sent {task.Result} byte(s)!");
    }

    /// <summary>
    /// Receive data
    /// </summary>
    /// <returns>Received data</returns>
    public override async Task<byte[]> Receive()
    {
        var buffer = new byte[16384];
        if (Socket is not { Connected: true })
        {
            return [];
        }

        var received = await Socket.ReceiveAsync(buffer);
        if (received == 0)
        {
            return [];
        }

        var msg = buffer.AsSpan()[..received].ToArray();
        Debug.Print($"TcpClient: received {msg.Length} bytes");
        ReceivedMessages.Add(msg);
        return msg;
    }

    public override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        CancellationTokenSource.Cancel();

        try
        {
            Socket?.Close();
            Socket?.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }
}