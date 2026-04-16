// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Diagnostics;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for TCP
/// </summary>
public class TcpIpServerSocketProxy : TcpIpSocketProxyBase
{
    private readonly byte[] _tmp = new byte[1];
    public readonly ITcpIpListenerManager TcpIpListenerManager;
    private Socket? _listener;
    private bool _isBound;

    /// <summary>
    /// Default ctor
    /// </summary>
    public TcpIpServerSocketProxy(ITcpIpListenerManager tcpIpListenerManager)
    {
        TcpIpListenerManager = tcpIpListenerManager;
    }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected => _isBound;
    //{
    //    get
    //    {
    //        // Replacement for Socket.Connected. See sample at the end of https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connected?redirectedfrom=MSDN&view=net-7.0#System_Net_Sockets_Socket_Connected

    //        try
    //        {
    //            if (_listener == null)
    //            {
    //                return false;
    //            }

    //            // This is how you can determine whether a socket is still connected.
    //            var blockingState = _listener.Blocking;
    //            try
    //            {
    //                _listener.Blocking = false;
    //                _listener.Send(_tmp, 0, 0);
    //                //Console.WriteLine("Connected!");
    //            }
    //            catch (SocketException e)
    //            {
    //                // 10035 == WSAEWOULDBLOCK
    //                if (e.NativeErrorCode.Equals(10035))
    //                {
    //                    // Still connected, but the send would block;
    //                }
    //                else
    //                {
    //                    // Disconnected
    //                    return false;
    //                }
    //            }
    //            finally
    //            {
    //                _listener.Blocking = blockingState;
    //            }

    //            return true;
    //        }
    //        catch //(Exception e)
    //        {
    //            return false;
    //        }
    //    }
    //}

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
            Trace.TraceInformation($"TcpServerSocket: sent {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                Debug.Print(socketException.ToString());

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
            Debug.Print(e.ToString());
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
            Trace.TraceInformation($"TcpServerSocket: sent {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                Debug.Print(socketException.ToString());

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
            Debug.Print(e.ToString());
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
            return _listener != null && _listener.Poll(1, SelectMode.SelectRead);
        }
        catch (Exception e)
        {
            Logger?.LogError("Polling failed", e);
            return false;
        }
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        _isBound = false;
        CancellationTokenSource = new CancellationTokenSource();

        if (_listener != null)
        {
            TcpIpListenerManager.UnregisterListener(_listener, AcceptDelegate);
        }
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
        await Task.Run(() =>
        {
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
            catch (Exception e)
            {
                var s = e.ToString();
                Trace.TraceError(s);
                // Do nothing
            }
            finally
            {
                Socket = null;
            }

            Debug.Print($"Server: port {Port}");
            _listener = TcpIpListenerManager.RegisterListener(Port, AcceptDelegate);
            _isBound = _listener != null;
        });
    }

    private bool AcceptDelegate(Socket clientSocket)
    {
        // ToDo: check remote IP address
        Socket = clientSocket;
        return true;
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
            Trace.TraceInformation($"TcpServerSocket: received {buffer.Length} bytes");
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
            return 0;
        }

        try
        {
            var result = await Socket.ReceiveAsync(buffer, SocketFlags.None, CancellationTokenSource.Token);
            Trace.TraceInformation($"TcpServerSocket received {buffer.Length} bytes");
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

    ///// <summary>
    ///// Receive data from the socket
    ///// </summary>
    ///// <param name="buffer">Byte array to store the received byte data in</param>
    ///// <param name="offset">Offset</param>
    ///// <param name="expectedBytesLength">Expected length of the byte data received</param>
    ///// <returns>Number of bytes received</returns>
    //public override async Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength)
    //{
    //    if (Socket is not { Connected: true })
    //    {
    //        return 0;
    //    }

    //    try
    //    {
    //        var result = await Socket.ReceiveAsync(buffer, CancellationTokenSource.Token);
    //        //SendEndPoint = result.RemoteEndPoint;

    //        //Debug.Print($"UDPClient: received {result.Length} bytes from {SendEndPoint}");

    //        result.Buffer.AsSpan().Slice(offset, expectedBytesLength).CopyTo(buffer);
    //        return expectedBytesLength;
    //    }
    //    catch (SocketException socketException)
    //    {
    //        if (socketException.ErrorCode != 10054)
    //        {
    //            Debug.Print(socketException.ToString());
    //        }

    //        return 0;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Print(e.ToString());
    //        return 0;
    //    }
    //}

    ///// <summary>
    ///// Send bytes 
    ///// </summary>
    ///// <param name="bytesToSend">Byte array to send</param>
    ///// <param name="offset">Offset</param>
    ///// <param name="messageBytesLength">Number of message bytes length to send</param>
    ///// <returns></returns>
    //public override Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
    //{
    //    if (Socket == null)
    //    {
    //        return Task.FromResult(0);
    //    }
    //    return !Socket.Connected ? Task.FromResult(0) : Socket.SendAsync(bytesToSend, offset, messageBytesLength);
    //}

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

        if (_listener != null)
        {
            TcpIpListenerManager.UnregisterListener(_listener, AcceptDelegate);
        }

        IsDisposed = true;
        Socket?.Close();
        Socket?.Dispose();
        Socket = null;
    }
}