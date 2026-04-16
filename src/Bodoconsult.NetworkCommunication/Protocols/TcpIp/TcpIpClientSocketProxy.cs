// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for a TCP client
/// </summary>
public class TcpIpClientSocketProxy : TcpIpSocketProxyBase
{
    private readonly byte[] _tmp = new byte[1];

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
                    Socket.Blocking = false;
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
            Trace.TraceInformation($"TcpClientSocket: Sent {bytesToSend.Length} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                var s = socketException.ToString();
                Trace.TraceError(s);
            }
            else
            {
                Logger?.LogDebug("No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger?.LogError("Sending failed", e);
            var s = e.ToString();
            Trace.TraceError(s);
            return 0;
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override async ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.SendAsync(bytesToSend, CancellationTokenSource.Token);
            Trace.TraceInformation($"TcpClientSocket: Sent {bytesToSend.Length} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                var s = socketException.ToString();
                Trace.TraceError(s);
            }
            else
            {
                Logger?.LogDebug("No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger?.LogError("Sending failed", e);
            var s = e.ToString();
            Trace.TraceError(s);
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
            Logger?.LogError("Polling failed", e);
            var s = e.ToString();
            Trace.TraceError(s);
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
        }
        catch (Exception e)
        {
            var s = e.ToString();
            Trace.TraceError(s);
        }
    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(byte[] buffer)
    {
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.ReceiveAsync(buffer, CancellationTokenSource.Token);
            Trace.TraceInformation($"TcpClientSocket: received {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Receiving failed", socketException);
                var s = socketException.ToString();
                Trace.TraceError(s);
            }
            else
            {
                Logger?.LogDebug("No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger?.LogError("Receiving failed", e);
            var s = e.ToString();
            Trace.TraceError(s);
            return 0;
        }
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(Memory<byte> buffer)
    {
        if (Socket is not { Connected: true })
        {
            return Task.FromResult(0).GetAwaiter().GetResult();
        }
        try
        {
            var result = await Socket.ReceiveAsync(buffer, SocketFlags.None, CancellationTokenSource.Token);
            Trace.TraceInformation($"TcpClientSocket: received {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                var s = socketException.ToString();
                Trace.TraceError(s);
            }

            return 0;
        }
        catch (Exception e)
        {
            var s = e.ToString();
            Trace.TraceError(s);
            return 0;
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
        CancellationTokenSource.Cancel();
        IsDisposed = true;
        Socket?.Close();
        Socket?.Dispose();
        Socket = null;
    }
}