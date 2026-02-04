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
public class AsyncUdpSocketProxy : UpdSocketProxyBase
{
    private readonly byte[] _tmp = new byte[1];

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint EndPoint;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint SendEndPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    public AsyncUdpSocketProxy(bool isServer)
    {
        IsServer = isServer;
    }

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public UdpClient UdpClient { get; protected set; }

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
                if (UdpClient.Client is not { Connected: true })
                {
                    return false;
                }

                // This is how you can determine whether a socket is still connected.
                var blockingState = UdpClient.Client.Blocking;
                try
                {
                    UdpClient.Client.Blocking = false;
                    UdpClient.Client.Send(_tmp, 0, 0);
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
                    UdpClient.Client.Blocking = blockingState;
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
                return UdpClient.Client is not { Connected: true } ? 0 : UdpClient.Available;
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
        return !UdpClient.Client.Connected ? Task.FromResult(0) : UdpClient.SendAsync(bytesToSend, bytesToSend.Length);
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        return !UdpClient.Client.Connected ? new ValueTask<int>(0) : UdpClient.SendAsync(bytesToSend);
    }

    /// <summary>
    /// Shut the socket down
    /// </summary>
    public override void Shutdown()
    {
        UdpClient.Client.Shutdown(SocketShutdown.Both);
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        UdpClient.Close();
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

            if (ClientPort == 0)
            {
                ClientPort = Port;
            }

            try
            {
                UdpClient = new UdpClient();
                UdpClient.ExclusiveAddressUse = false;
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.ReceiveTimeout = ReceiveTimeout;
                UdpClient.Client.SendTimeout = SendTimeout;

                if (IsServer)
                {
                    var endPoint1 = new IPEndPoint(IPAddress.Any, Port);
                    UdpClient.Client.Bind(endPoint1);

                    EndPoint = new IPEndPoint(IpAddress, Port);
                    SendEndPoint = new IPEndPoint(IpAddress, ClientPort);
                }
                else
                {
                    var ep1 = new IPEndPoint(IPAddress.Any, ClientPort);
                    UdpClient.Client.Bind(ep1);

                    EndPoint = new IPEndPoint(IpAddress, ClientPort);
                    SendEndPoint = new IPEndPoint(IpAddress, Port);
                }
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
        if (!UdpClient.Client.Connected)
        {
            Task.FromResult(0);
        }

        var result = UdpClient.ReceiveAsync();
        Buffer.BlockCopy(result.Result.Buffer, 0, buffer, 0, buffer.Length);
        return Task.FromResult(result.Result.Buffer.Length);
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(Memory<byte> buffer)
    {
        if (!UdpClient.Client.Connected)
        {
            await Task.FromResult(0);
        }

        var result = await UdpClient.ReceiveAsync();
        result.Buffer.CopyTo(buffer);
        return result.Buffer.Length;
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
        if (!UdpClient.Client.Connected)
        {
            return Task.FromResult(0);
        }

        var result = Task.Run(async ()=>
        {
            var result = await UdpClient.ReceiveAsync();
            Buffer.BlockCopy(result.Buffer, offset, buffer, 0, buffer.Length -offset);
            return result.Buffer.Length;
        });

        return result;
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
        return !UdpClient.Client.Connected ? Task.FromResult(0) : UdpClient.Client.SendAsync(bytesToSend, offset, messageBytesLength);
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