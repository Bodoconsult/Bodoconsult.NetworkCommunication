// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
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
        LoggerId = "TcpServer";

        Debug.Print($"{LoggerId}: port {port}");

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

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    public override void StartReceiverLoop()
    {
        // Do nothing
    }

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public override async Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(Socket, $"TcpClient: Socket is null");

            Debug.Print($"TcppClient: Error: ReceiverLoop started");

            waitForLoopStarted.Set();

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                var result = 0;
                var buffer = new byte[MaxPacketSize].AsMemory();
                try
                {
                    result = await Socket.ReceiveAsync(buffer, SocketFlags.None, CancellationTokenSource.Token);
                    Debug.Print($"TcppClient: Error: received {result} bytes");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Debug.Print($"TcppClient: Error: Receiving failed ", e);
                }

                if (result == 0)
                {
                    await Task.Delay(5);
                }

                ReceivedMessages.Add(buffer[..result].ToArray());
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception e)
        {
            Debug.Print($"TcppClient: Error: Receiver loop failed", e);
        }
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

            AutoResetEvent wait = new(false);

            // Start receive loop now
            Task.Run(async () =>
            {
                await ReceiverLoop(wait);
            });

            wait.WaitOne(100);
        }
        catch (Exception e)
        {
            Debug.Print($"{LoggerId}{e.Message}");
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

        Debug.Print($"{LoggerId}sent {task.Result} byte(s)!");
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