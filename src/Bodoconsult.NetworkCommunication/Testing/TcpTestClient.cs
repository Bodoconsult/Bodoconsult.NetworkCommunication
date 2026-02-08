// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpTestClient :   ITcpIpDevice
{
    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Local port</param>
    public TcpTestClient(IPAddress ipAddress, int port)
    {
        Debug.Print($"Client: port {port}");

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout
        };
        _socket.ExclusiveAddressUse = false;
        _socket.NoDelay = true;
        _socket.Blocking = false;

        _endPoint = new IPEndPoint(ipAddress, port);
    }

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Current cancellation token
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    /// <summary>
    /// Start the server mode
    /// </summary>
    public void Start()
    {
        //try
        //{

        _socket.ConnectAsync(_endPoint).Wait(5000);

        //}
        //catch (Exception e)
        //{

        //}
    }

    /// <summary>
    /// Reset the client socket if necessary
    /// </summary>
    public void ResetClientSocket()
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
    public void Send(byte[] data)
    {
        var task = _socket.SendToAsync(data, _endPoint);
        task.Wait(CancellationTokenSource.Token);

        Debug.Print($"TcpClient: sent {task.Result} byte(s)!");
    }

    public virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        CancellationTokenSource.Cancel();

        try
        {
            _socket?.Close();
            _socket?.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}