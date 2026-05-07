// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

// https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
// https://dev.to/chakewitz/c-networking-raw-sockets-tcp-and-udp-programming-46oc
// https://learn.microsoft.com/de-de/dotnet/framework/network-programming/using-udp-services
// https://enclave.io/high-performance-udp-sockets-net6/

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast
/// </summary>
public class UdpClientSocketProxy : BaseUpdSocketProxy
{
    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public UdpClientSocketProxy(IAppLoggerProxy logger) : base(logger)
    { }

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    public IPEndPoint? SendEndPoint { get; set; }

    private bool _isBound;

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public UdpClient? UdpClient { get; protected set; }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected => _isBound;

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    public override int BytesAvailable
    {
        get
        {
            try
            {
                return UdpClient?.Available ?? 0;
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
    public override Task<int> Send(byte[] bytesToSend)
    {
        throw new NotSupportedException();

        //if (UdpClient == null)
        //{
        //    return 0;
        //}

        //ArgumentNullException.ThrowIfNull(SendEndPoint);

        //try
        //{
        //    var result = await UdpClient.SendAsync(bytesToSend, bytesToSend.Length, SendEndPoint);

        //    //var result = await UdpClient.Client.SendToAsync(bytesToSend, SendEndPoint);
        //    Trace.TraceInformation($"{LoggerId}sent {result} bytes to {SendEndPoint}");
        //    return result;
        //}
        //catch (SocketException socketException)
        //{
        //    if (socketException.ErrorCode != 10054)
        //    {
        //        Logger?.LogError("Sending failed", socketException);
        //        var s = socketException.ToString();
        //        Trace.TraceError($"{LoggerId}{s}");
        //    }
        //    else
        //    {
        //        Logger?.LogDebug("No connection");
        //    }

        //    return 0;
        //}
        //catch (Exception e)
        //{
        //    Logger?.LogError("Sending failed", e);
        //    var s = e.ToString();
        //    Trace.TraceError($"{LoggerId}{s}");
        //    return 0;
        //}
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        throw new NotSupportedException();

        //if (UdpClient == null)
        //{
        //    return 0;
        //}

        //ArgumentNullException.ThrowIfNull(SendEndPoint);

        //try
        //{
        //    //var result = await UdpClient.Client.SendToAsync(bytesToSend, SendEndPoint);

        //    var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint).AsTask();
        //    Trace.TraceInformation($"{LoggerId}sent {result} bytes");
        //    return result;
        //}
        //catch (SocketException socketException)
        //{
        //    if (socketException.ErrorCode != 10054)
        //    {
        //        Logger?.LogError("Sending failed", socketException);
        //        var s = socketException.ToString();
        //        Trace.TraceError($"{LoggerId}{s}");
        //    }
        //    else
        //    {
        //        Logger?.LogDebug("No connection");
        //    }

        //    return 0;
        //}
        //catch (Exception e)
        //{
        //    Logger?.LogError("Sending failed", e);
        //    var s = e.ToString();
        //    Trace.TraceError($"{LoggerId}{s}");
        //    return 0;
        //}
    }

    ///// <summary>
    ///// Send bytes 
    ///// </summary>
    ///// <param name="bytesToSend">Byte array to send</param>
    ///// <param name="offset">Offset</param>
    ///// <param name="messageBytesLength">Number of message bytes length to send</param>
    ///// <returns></returns>
    //public override Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
    //{
    //    if (UdpClient == null)
    //    {
    //        return Task.FromResult(0);
    //    }

    //    var datagram = bytesToSend.AsMemory().Slice(offset, messageBytesLength);
    //    return UdpClient.Client.SendAsync(datagram.ToArray());
    //}

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        _isBound = false;
        CancellationTokenSource.Cancel();

        if (UdpClient == null)
        {
            return;
        }

        UdpClient.Client.Shutdown(SocketShutdown.Both);
        UdpClient.Close();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {
        ArgumentNullException.ThrowIfNull(IpAddress);

        await Task.Run(() =>
        {
            try
            {
                if (UdpClient != null)
                {
                    UdpClient.Client.Shutdown(SocketShutdown.Both);
                    UdpClient.Close();
                    UdpClient.Dispose();

                    CancellationTokenSource = new CancellationTokenSource();
                }
            }
            catch // (Exception ex)
            {
                // Do nothing
            }
            finally
            {
                UdpClient = null;
            }

            try
            {
                UdpClient = new UdpClient(Port);
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.Blocking = false;

                EndPoint = new IPEndPoint(IpAddress, Port);
                SendEndPoint = EndPoint;

                //UdpClient.Connect(EndPoint);
                Logger.LogInformation($"{LoggerId}connected to IPAddress.Any:{Port}");
                _isBound = true;
            }
            catch (Exception e)
            {
                // ToDo: add logging
                Logger.LogError($"{LoggerId}{e}");
                _isBound = false;
                throw;
            }
        });
    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(byte[] buffer)
    {
        if (UdpClient == null || UdpClient.Available == 0)
        {
            return await Task.FromResult(0);
        }

        try
        {
            var result = await UdpClient.ReceiveAsync(CancellationTokenSource.Token);
            result.Buffer.CopyTo(buffer, 0);
            Logger.LogInformation($"{LoggerId}received {result.Buffer.Length} bytes");
            return result.Buffer.Length;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger.LogError($"{LoggerId}Receiving failed", socketException);
            }
            else
            {
                Logger.LogDebug($"{LoggerId}No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Receiving failed", e);
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
        if (UdpClient == null || UdpClient.Available == 0)
        {
            return await Task.FromResult(0);
        }

        try
        {
            var result = await UdpClient.ReceiveAsync(CancellationTokenSource.Token).AsTask();
            result.Buffer.CopyTo(buffer);
            Logger.LogInformation($"{LoggerId}received {result.Buffer.Length} bytes");
            return result.Buffer.Length;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger.LogError($"{LoggerId}Receiving failed", socketException);
            }
            else
            {
                Logger.LogDebug($"{LoggerId}No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Receiving failed", e);
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
            return UdpClient != null && UdpClient.Client.Poll(1, SelectMode.SelectRead);
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Polling failed", e);
            return false;
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
    //    if (UdpClient == null)
    //    {
    //        return 0;
    //    }

    //    try
    //    {
    //        var result = await UdpClient.ReceiveAsync(CancellationTokenSource.Token);
    //        //SendEndPoint = result.RemoteEndPoint;

    //        //Trace.TraceInformation($"UDPClient: received {result.Length} bytes from {SendEndPoint}");

    //        result.Buffer.AsSpan().Slice(offset, expectedBytesLength).CopyTo(buffer);
    //        return expectedBytesLength;
    //    }
    //    catch (SocketException socketException)
    //    {
    //        if (socketException.ErrorCode != 10054)
    //        {
    //            Trace.TraceInformation(socketException.ToString());
    //        }

    //        return 0;
    //    }
    //    catch (Exception e)
    //    {
    //        Trace.TraceInformation(e.ToString());
    //        return 0;
    //    }
    //}

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel();
        IsDisposed = true;
        UdpClient?.Close();
        UdpClient?.Dispose();
        UdpClient = null;
        _isBound = false;
    }
}