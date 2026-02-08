// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpTestServer : ITcpIpDevice
{
    private readonly Socket _listener;
    private Socket _clientSocket;
    private readonly IPEndPoint _endPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public TcpTestServer(IPAddress ipAddress, int port)
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout
        };

        _listener.NoDelay = true;
        _listener.Blocking = false;

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
        if (_clientSocket != null)
        {
            return;
        }

        try
        {
            _clientSocket = _listener.EndAccept(ar);
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
        }
    }

    /// <summary>
    /// Reset the client socket if necessary
    /// </summary>
    public void ResetClientSocket()
    {
        if (_clientSocket == null)
        {
            return;
        }
        _clientSocket.Close();
        _clientSocket = null;
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public void Send(byte[] data)
    {
        var task = _clientSocket.SendAsync(data);
        task.Wait(CancellationTokenSource.Token);

        Debug.Print($"TcpServer: sent {task.Result} byte(s)!");
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

            _clientSocket?.Close();
            _clientSocket?.Dispose();
        }
        catch
        {
            // Do nothing
        }

        try
        {
            _listener?.Close();
            _listener?.Dispose();
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