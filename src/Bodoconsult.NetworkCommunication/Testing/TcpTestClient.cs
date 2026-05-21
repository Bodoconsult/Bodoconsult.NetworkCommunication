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
        LoggerId = "TcpClient";

        Debug.Print($"{LoggerId}: port {port}");

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
    /// Start the receiver loop
    /// </summary>
    public override void StartReceiverLoop()
    {
        AutoResetEvent wait = new(false);

        // Start receive loop now
        Task.Run(async () =>
        {
            await ReceiverLoop(wait);
        });

        wait.WaitOne(100);
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
            ArgumentNullException.ThrowIfNull(Socket, $"{LoggerId}Socket is null");

            Debug.Print($"{LoggerId}Error: ReceiverLoop started");

            waitForLoopStarted.Set();

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                var result = 0;
                var buffer = new byte[MaxPacketSize].AsMemory();
                try
                {
                    result = await Socket.ReceiveAsync(buffer, SocketFlags.None, CancellationTokenSource.Token);
                    Debug.Print($"{LoggerId}Error: received {result} bytes");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Debug.Print($"{LoggerId}Error: Receiving failed ", e);
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
            Debug.Print($"{LoggerId}Error: Receiver loop failed", e);
        }
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

        //Debug.Print($"TcpClient: sent {task.Result} byte(s)!");
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
            Socket?.Close();
            Socket?.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }
}