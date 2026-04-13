// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

// https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
// https://dev.to/chakewitz/c-networking-raw-sockets-tcp-and-udp-programming-46oc
// https://learn.microsoft.com/de-de/dotnet/framework/network-programming/using-udp-services
// https://enclave.io/high-performance-udp-sockets-net6/

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast server
/// </summary>
public class UdpServerSocketProxy : UpdSocketProxyBase
{
    //private readonly byte[] _tmp = new byte[1];

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? SendEndPoint;

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
                //Debug.Print($"Bytes available: {UdpClient?.Client.Available ?? 0}");
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
                
                // The following three lines allow multiple clients on the same PC
                //UdpClient.ExclusiveAddressUse = false;
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                EndPoint = new IPEndPoint(IPAddress.Any, Port);
                _isBound = true;

            }
            catch (Exception e)
            {
                // ToDo: add logging
                Debug.Print(e.ToString());
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
        if (UdpClient == null)
        {
            return Task.FromResult(0);
        }

        var received = Task.Run(() =>
        {
            var result = UdpClient.Receive(ref SendEndPoint);
            Buffer.BlockCopy(result, 0, buffer, 0, result.Length);
            return Task.FromResult(result.Length);
        });

        return received;
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(Memory<byte> buffer)
    {
        if (UdpClient == null)
        {
            return 0;
        }

        var received = await Task.Run(() =>
        {
            var result = UdpClient.Receive(ref SendEndPoint);
            result.CopyTo(buffer);
            return Task.FromResult(result.Length);
        });

        return received;
    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <param name="offset">Offset</param>
    /// <param name="expectedBytesLength">Expected length of the byte data received</param>
    /// <returns>Number of bytes received</returns>
    public override Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength)
    {
        if (UdpClient == null)
        {
            return Task.FromResult(0);
        }

        var received = Task.Run(() =>
        {
            var result = UdpClient.Receive(ref SendEndPoint);
            Buffer.BlockCopy(result, offset, buffer, 0, result.Length - offset);
            return Task.FromResult(result.Length);
        });

        return received;
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override Task<int> Send(byte[] bytesToSend)
    {
        return UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any) ? Task.FromResult(0) : UdpClient.SendAsync(bytesToSend, bytesToSend.Length, SendEndPoint);
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        return UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any) ? ValueTask.FromResult(0) :  UdpClient.SendAsync(bytesToSend, SendEndPoint);
    }

    /// <summary>
    /// Send bytes 
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    /// <param name="offset">Offset</param>
    /// <param name="messageBytesLength">Number of message bytes length to send</param>
    /// <returns></returns>
    public override Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
    {
        return UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any) ? Task.FromResult(0) : UdpClient.Client.SendToAsync(bytesToSend, offset, messageBytesLength, SocketFlags.None, SendEndPoint);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        IsDisposed = true;
        UdpClient?.Close();
        UdpClient?.Dispose();
        UdpClient = null;
    }
}