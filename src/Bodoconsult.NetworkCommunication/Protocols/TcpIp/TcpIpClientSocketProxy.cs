// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for a TCP client
/// </summary>
public class TcpIpClientSocketProxy : BaseTcpIpSocketProxy
{
    private readonly byte[] _tmp = new byte[1];
    private readonly StreamPipeline _pipeline;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public TcpIpClientSocketProxy(IAppLoggerProxy logger) : base(logger, new StreamPipeline())
    {
        _pipeline = (StreamPipeline)ReceiverPipeline;
        _pipeline.BufferSize = MaxPacketSize;
    }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected
    {
        get
        {
            // Replacement for Socket.Connected. See sample at the end of https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connected?redirectedfrom=MSDN&view=net-7.0#System_Net_Sockets_Socket_Connected

            try
            {
                if (Socket is not { Connected: true })
                {
                    return false;
                }

                // This is how you can determine whether a socket is still connected.
                var blockingState = Socket.Blocking;
                try
                {

                    Socket.Send(_tmp, 0, 0);
                    //Console.WriteLine("Connected!");
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (e.NativeErrorCode.Equals(10035))
                    {
                        // Still connected, but the send would block;
                    }
                    else
                    {
                        // Disconnected
                        return false;
                    }
                }
                finally
                {
                    Socket.Blocking = blockingState;
                }

                return true;
            }
            catch //(Exception e)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    public override int BytesAvailable
    {
        get
        {
            try
            {
                return Socket is not { Connected: true } ? 0 : Socket.Available;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override async Task<int> Send(byte[] bytesToSend)
    {
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.SendAsync(bytesToSend, CancellationTokenSource.Token);
            Logger.LogDebug($"{LoggerId}sent {bytesToSend.Length} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger.LogError($"{LoggerId}Sending failed", socketException);
            }
            else
            {
                Logger.LogDebug($"{LoggerId}No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Sending failed", e);
            return 0;
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override async Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.SendAsync(bytesToSend, CancellationTokenSource.Token).AsTask();
            Logger.LogDebug($"{LoggerId}sent {bytesToSend.Length} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger.LogError($"{LoggerId}Sending failed", socketException);
            }
            else
            {
                Logger.LogDebug($"{LoggerId}No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Sending failed", e);
            return 0;
        }
    }

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public override bool Poll()
    {
        try
        {
            return Socket != null && Socket.Poll(1, SelectMode.SelectRead);
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Polling failed", e);
            return false;
        }
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        CancellationTokenSource = new CancellationTokenSource();

        if (Socket == null)
        {
            return;
        }

        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {
        ArgumentNullException.ThrowIfNull(IpAddress);

        try
        {
            if (Socket != null)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket?.Dispose();

                CancellationTokenSource = new CancellationTokenSource();
            }
        }
        catch // (Exception ex)
        {
            // Do nothing
        }
        finally
        {
            Socket = null;
        }

        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout,
            NoDelay = true,
            Blocking = false
        };
        Socket.SetSocketKeepAliveValues(7200000, 1000);
        Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        EndPoint ep = new IPEndPoint(IpAddress, Port);

        try
        {
            await Socket.ConnectAsync(ep);
            Logger.LogInformation($"{LoggerId}connected to {IpAddress}:{Port}");
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}{e}");
        }
    }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    public override void StartReceiverLoop(SocketReceivedDataDelegate2 socketReceivedDataDelegate)
    {
        SocketReceivedDataDelegate = socketReceivedDataDelegate;

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
            ArgumentNullException.ThrowIfNull(SocketReceivedDataDelegate, $"{LoggerId}SocketReceivedDataDelegate is null");

            Logger.LogInformation($"{LoggerId}ReceiverLoop started");

            waitForLoopStarted.Set();

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                var buffer = _pipeline.GetBuffer();

                try
                {
                    var pipeLen = _pipeline.Buffer.Length;
                    var result = await Socket.ReceiveAsync(buffer.Memory, SocketFlags.None, CancellationTokenSource.Token);

                    _pipeline.AddMemory(buffer, result);
                    await SocketReceivedDataDelegate.Invoke();

                    Logger.LogInformation(
                        $"{LoggerId}TCPC: received {result} bytes: buffer before {pipeLen} bytes after {_pipeline.Buffer.Length} bytes");
                }
                catch (OperationCanceledException)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    break;
                }
                catch (SocketException se)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    Logger.LogError($"{LoggerId}Receiving failed: socket error {se.ErrorCode}: {se.Message}");
                }
                catch (Exception e)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    Logger.LogError($"{LoggerId}Receiving failed ", e);
                }

                //if (result == 0)
                //{
                //    await Task.Delay(5);
                //}

                //AsyncHelper.FireAndForget(() =>
                //{
                //    try
                //    {
                //        SocketReceivedDataDelegate.Invoke(buffer[..result].ToArray());
                //    }
                //    catch (Exception e)
                //    {
                //        Logger.LogError($"{LoggerId}Forwarding received data failed", e);
                //    }
                //});
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (SocketException se)
        {
            Logger.LogError($"{LoggerId}Receiver loop failed: socket error {se.ErrorCode}: {se.Message}");
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Receiver loop failed", e);
        }
    }

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public Socket? Socket { get; protected set; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel(false);
        IsDisposed = true;
        Socket?.Close();
        Socket?.Dispose();
        Socket = null;
    }
}