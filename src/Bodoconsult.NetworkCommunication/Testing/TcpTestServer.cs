// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpTestServer : IDisposable
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
        // Creation TCP/IP Socket using
        // Socket Class Constructor
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout
        };

        // Establish the local endpoint
        // for the socket. Dns.GetHostName
        // returns the name of the host
        // running the application.
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
        // network address to the Server Socket
        // All client that will connect to this
        // Server Socket must know this network
        // Address
        _listener.Bind(_endPoint);

        //// Using Listen() method we create
        //// the Client list that will want
        //// to connect to Server
        _listener.Listen(10);


        //}
        //catch (Exception e)
        //{

        //}
    }


    /// <summary>
    /// Wait for connections
    /// </summary>
    /// <returns></returns>
    public async Task WaitForConnections()
    {
        while (!CancellationTokenSource.Token.IsCancellationRequested)
        {

            // Suspend while waiting for
            // incoming connection Using
            // Accept() method the server
            // will accept connection of client
            if (_clientSocket == null)
            {
                try
                {
                    _clientSocket = await _listener.AcceptAsync();
                }
                catch
                {
                    _clientSocket = null;
                }

            }

            await Task.Delay(51, CancellationTokenSource.Token);
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
        //if (clientSocket == null)
        //{
        //    var taskCs = _listener.Socket.AcceptAsync();
        //    taskCs.Wait(CancellationToken);
        //    clientSocket = taskCs.Result;
        //}

        var task = _clientSocket.SendAsync(data);
        task.Wait(CancellationTokenSource.Token);

        Debug.Print($"TcpServer: sent {task.Result} byte(s)!");
    }

    protected virtual void Dispose(bool disposing)
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