// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

// https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
// https://dev.to/chakewitz/c-networking-raw-sockets-tcp-and-udp-programming-46oc
// https://learn.microsoft.com/de-de/dotnet/framework/network-programming/using-udp-services
// https://enclave.io/high-performance-udp-sockets-net6/

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast server only sending to one client. No receiving of data
/// </summary>
public class UdpServerSocketProxy : BaseUpdSocketProxy
{
    //private readonly byte[] _tmp = new byte[1];

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

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
                //Trace.TraceInformation($"Bytes available: {UdpClient?.Client.Available ?? 0}");
                return UdpClient?.Client.Available ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }

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
        UdpClient = null;
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
                UdpClient = new UdpClient();

                // The following three lines allow multiple clients on the same PC
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.Blocking = false;

                EndPoint = new IPEndPoint(IpAddress, Port);
                SendEndPoint = EndPoint;
                Trace.TraceInformation($"{LoggerId}sending to {IpAddress}:{Port}");

                _isBound = true;
            }
            catch (Exception e)
            {
                // ToDo: add logging
                var s = e.ToString();
                Trace.TraceError($"{LoggerId}{s}");
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
    public override Task<int> Receive(byte[] buffer)
    {
        throw new NotSupportedException();

        //if (UdpClient == null)
        //{
        //    return await Task.FromResult(0);
        //}

        //ArgumentNullException.ThrowIfNull(EndPoint);

        //try
        //{
        //    if (UdpClient.Available == 0)
        //    {
        //        return 0;
        //    }

        //    var result = await UdpClient.ReceiveAsync();
        //    SendEndPoint = result.RemoteEndPoint;

        //    //var result = await UdpClient.Client.ReceiveFromAsync(buffer, ep);

        //    Trace.TraceInformation($"{LoggerId}received {result.Buffer.Length} bytes");
        //    Buffer.BlockCopy(result.Buffer, 0, buffer, 0, result.Buffer.Length);
        //    return result.Buffer.Length;
        //}
        //catch (SocketException socketException)
        //{
        //    switch (socketException.ErrorCode)
        //    {
        //        case 10054:
        //        case 995:
        //            Trace.TraceInformation($"{LoggerId}no connection");
        //            Logger?.LogDebug("No connection");
        //            break;
        //        default:
        //            Logger?.LogError("Receiving failed", socketException);
        //            var s = socketException.ToString();
        //            Trace.TraceError($"{LoggerId}{s}");
        //            break;
        //    }
        //    return 0;
        //}
        //catch (Exception e)
        //{
        //    Logger?.LogError("Receiving failed", e);
        //    var s = e.ToString();
        //    Trace.TraceError($"{LoggerId}{s}");
        //    return 0;
        //}
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override Task<int> Receive(Memory<byte> buffer)
    {
        throw new NotSupportedException();

        //if (UdpClient == null)
        //{
        //    return 0;
        //}

        //ArgumentNullException.ThrowIfNull(EndPoint);

        //try
        //{
        //    var result = await UdpClient.ReceiveAsync(CancellationTokenSource.Token);
        //    SendEndPoint = result.RemoteEndPoint;

        //    //Trace.TraceInformation($"UDPClient: received {result.Length} bytes from {SendEndPoint}");

        //    result.Buffer.CopyTo(buffer);
        //    Trace.TraceInformation($"{LoggerId}received {result.Buffer.Length} bytes");
        //    return result.Buffer.Length;
        //}
        //catch (SocketException socketException)
        //{
        //    switch (socketException.ErrorCode)
        //    {
        //        case 10054:
        //        case 995:
        //            Logger?.LogDebug("No connection");
        //            Trace.TraceInformation($"{LoggerId}no connection");
        //            break;
        //        default:
        //            Logger?.LogError("Receiving failed", socketException);
        //            var s = socketException.ToString();
        //            Trace.TraceError($"{LoggerId}{s}");
        //            break;
        //    }
        //    return 0;
        //}
        //catch (Exception e)
        //{
        //    Logger?.LogError("Receiving failed", e);
        //    var s = e.ToString();
        //    Trace.TraceError($"{LoggerId}{s}");
        //    return 0;
        //}
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
            Logger?.LogError("Polling failed", e);
            var s = e.ToString();
            Trace.TraceError($"{LoggerId}{s}");
            return false;
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override async Task<int> Send(byte[] bytesToSend)
    {
        if (UdpClient == null || SendEndPoint == null)
        {
            Trace.TraceWarning($"{LoggerId}UdpClient is null or SendEndPoint is null or address IPAddress.Any. No client request before?");
            return 0;
        }

        try
        {
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token);
            Trace.TraceInformation($"{LoggerId}sent {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                var s = socketException.ToString();
                Trace.TraceError($"{LoggerId}{s}");
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
            Trace.TraceError($"{LoggerId}{s}");
            return 0;
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override async Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        if (UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any))
        {
            Trace.TraceWarning($"{LoggerId}UdpClient is null or SendEndPoint is null or address IPAddress.Any. No client request before?");
            return 0;
        }

        try
        {
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token).AsTask();
            Trace.TraceInformation($"{LoggerId}sent {result} bytes");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger?.LogError("Sending failed", socketException);
                var s = socketException.ToString();
                Trace.TraceError($"{LoggerId}{s}");
            }
            else
            {
                Trace.TraceInformation("No connection");
                Logger?.LogDebug("No connection");
            }

            return 0;
        }
        catch (Exception e)
        {
            Logger?.LogError("Sending failed", e);
            var s = e.ToString();
            Trace.TraceError($"{LoggerId}{s}");
            return 0;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel();
        IsDisposed = true;

        if (UdpClient == null)
        {
            return;
        }

        UdpClient.Client.Shutdown(SocketShutdown.Both);
        UdpClient.Close();
        UdpClient.Dispose();
        UdpClient = null;
    }
}