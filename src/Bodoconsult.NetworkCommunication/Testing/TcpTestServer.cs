// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpTestServer : BaseTcpIpDevice
{
    private readonly Socket _listener;

    private readonly IPEndPoint _endPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public TcpTestServer(IPAddress ipAddress, int port)
    {
        IsServer = true;

        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout
        };

        _listener.NoDelay = true;
        _listener.Blocking = false;

        _endPoint = new IPEndPoint(ipAddress, port);
        // Using Bind() method we associate a
        // network address to the server socket.
        // All client that will connect to this
        // server socket must know this network
        // Address
        _listener.Bind(_endPoint);

        //// Using Listen() method we create
        //// the client list that will want
        //// to connect to Server
        _listener.Listen(10);

        // Now wait for client connections
        _listener.BeginAccept(AcceptCallback, _listener);

        //}
        //catch (Exception e)
        //{

        //}
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request
        if (Socket != null)
        {
            return;
        }

        try
        {
            Socket = _listener.EndAccept(ar);
        }
        catch (Exception e)
        {
            Trace.TraceInformation(e.Message);
        }
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public override void Send(byte[] data)
    {
        if (Socket == null)
        {
            return;
        }

        var task = Socket.SendAsync(data);
        task.Wait(CancellationTokenSource.Token);

        Trace.TraceInformation($"TcpServer: sent {task.Result} byte(s)!");
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
        Trace.TraceInformation($"TcpServer: received {msg.Length} bytes");
        ReceivedMessages.Add(msg);
        return msg;
    }

    /// <summary>
    /// Dispose the instance
    /// </summary>
    /// <param name="disposing">Is disposing?</param>
    public override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        CancellationTokenSource.Cancel();

        try
        {
            if (Socket != null)
            {
                Socket.Close();
                Socket.Dispose();
            }
        }
        catch
        {
            // Do nothing
        }

        try
        {
            _listener.Close();
            _listener.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }

    ///// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}
}