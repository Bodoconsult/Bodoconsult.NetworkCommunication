// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

// https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
// https://dev.to/chakewitz/c-networking-raw-sockets-tcp-and-udp-programming-46oc
// https://learn.microsoft.com/de-de/dotnet/framework/network-programming/using-udp-services
// https://enclave.io/high-performance-udp-sockets-net6/

using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast
/// </summary>
public class UdpClientSocketProxy : UpdSocketProxyBase
{
    //private readonly byte[] _tmp = new byte[1];

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public UdpClient? UdpClient { get; protected set; }

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
        return UdpClient == null ? Task.FromResult(0) : UdpClient.SendAsync(bytesToSend, bytesToSend.Length);
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        return UdpClient?.SendAsync(bytesToSend) ?? ValueTask.FromResult(0);
    }

    ///// <summary>
    ///// Shut the socket down
    ///// </summary>
    //public override void Shutdown()
    //{
    //    UdpClient.Client.Shutdown(SocketShutdown.Both);
    //}

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
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
                UdpClient = new UdpClient();
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                EndPoint = new IPEndPoint(IpAddress, Port);
                UdpClient.Connect(EndPoint);

            }
            catch (Exception e)
            {
                // ToDo: add logging
                Console.WriteLine(e);
                throw;
            }
        });
    }

    ///// <summary>
    ///// Bind to an IP endpoint
    ///// </summary>
    ///// <param name="endpoint">IP endpoint</param>
    //public override void Bind(IPEndPoint endpoint)
    //{
    //    Socket?.Bind(endpoint);
    //}

    ///// <summary>
    ///// Listen
    ///// </summary>
    ///// <param name="backlog">The maximum length of pending messages queue</param>
    //public override void Listen(int backlog)
    //{
    //    Socket?.Listen(backlog);
    //}

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
            var result = UdpClient.Receive(ref EndPoint);
            Buffer.BlockCopy(result, 0, buffer, 0, buffer.Length);
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
            var result = UdpClient.Receive(ref EndPoint);
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
            var result = UdpClient.Receive(ref EndPoint);
            Buffer.BlockCopy(result, offset, buffer, 0, buffer.Length - offset);
            return Task.FromResult(result.Length);
        });

        return received;
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
        if (UdpClient == null)
        {
            return Task.FromResult(0);
        }

        var datagram = bytesToSend.AsMemory().Slice(offset, messageBytesLength);
        

        return UdpClient.Client.SendAsync(datagram.ToArray());
    }

    ///// <summary>
    ///// Poll data
    ///// </summary>
    ///// <returns>True, if data can be read, else false</returns>
    //public override bool Poll()
    //{
    //    return Socket.Poll(PollingTimeout, SelectMode.SelectRead);
    //}

    ///// <summary>
    ///// Send a file
    ///// </summary>
    ///// <param name="fileName">Full file path</param>
    //public override void SendFile(string fileName)
    //{
    //    Socket.SendFile(fileName);
    //}

    ///// <summary>
    ///// Prepare the answer of the socket for testing
    ///// </summary>
    ///// <param name="testData">Test data to use</param>
    //public override void PrepareAnswer(byte[] testData)
    //{
    //    // Do nothing
    //}


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