// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Tests.Infrastructure;

/// <summary>
/// Simple TCP/IP server for testing purposes
/// </summary>
public class TcpServer : IDisposable
{

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;


    private readonly Socket _listener;

    private Socket clientSocket;

    private readonly IPEndPoint _endPoint;

    public TcpServer(IPAddress ipAddress, int port)
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

    public CancellationToken CancellationToken { get; set; } = new(false);


    /// <summary>
    /// Start the server mode
    /// </summary>
    public void StartServer()
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
        while (!CancellationToken.IsCancellationRequested)
        {

            // Suspend while waiting for
            // incoming connection Using
            // Accept() method the server
            // will accept connection of client
            if (clientSocket == null)
            {
                try
                {
                    clientSocket = await _listener.AcceptAsync();
                }
                catch
                {
                    clientSocket = null;
                }

            }

            await Task.Delay(51, CancellationToken);
        }
    }

    /// <summary>
    /// Reset the client socket if necessary
    /// </summary>
    public void ResetClientSocket()
    {
        if (clientSocket == null)
        {
            return;
        }
        clientSocket.Close();
        clientSocket = null;
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

        var task = clientSocket.SendAsync(data);
        task.Wait(CancellationToken);

        Debug.Print($"TcpServer: sent {task.Result} byte(s)!");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        CancellationToken = new CancellationToken(true);

        try
        {

            clientSocket?.Close();
            clientSocket?.Dispose();
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