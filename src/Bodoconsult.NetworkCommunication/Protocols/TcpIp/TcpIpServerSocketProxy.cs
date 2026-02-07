// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for TCP
/// </summary>
public class TcpIpServerSocketProxy : TcpIpSocketProxyBase
{
    private readonly byte[] _tmp = new byte[1];

    private Socket _listener;

    ///// <summary>
    ///// Default ctor
    ///// </summary>
    //public TcpIpServerSocketProxy()
    //{ }

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
    public override Task<int> Send(byte[] bytesToSend)
    {
        return !Socket.Connected ? Task.FromResult(0) : Socket.SendAsync(bytesToSend);
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        return !Socket.Connected ? new ValueTask<int>(0) : Socket.SendAsync(bytesToSend, SocketFlags.None);
    }

    /// <summary>
    /// Shut the socket down
    /// </summary>
    public override void Shutdown()
    {
        Socket.Shutdown(SocketShutdown.Both);
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        Socket.Close();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {

        try
        {
            if (Socket != null)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket?.Dispose();
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


        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout,
            NoDelay = true,
            Blocking = false
        };
        _listener.SetSocketKeepAliveValues(7200000, 1000);

        // Bind listener to all IP addresses of the local network adapter
        EndPoint ep = new IPEndPoint(IPAddress.Any, Port);
        _listener.Bind(ep);

        // Now start listening
        _listener.Listen(1000);

        // Now begin
        _listener.BeginAccept(AcceptCallback, _listener);
    }

    /// <summary>
    /// Callback for accepting new connection
    /// </summary>
    /// <param name="ar"></param>
    private void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request
        _listener = (Socket)ar.AsyncState;

        if (_listener == null)
        {
            throw new ArgumentNullException(nameof(_listener));
        }

        if (Socket != null)
        {
            return;
        }

        try
        {
            Socket = _listener.EndAccept(ar);
        }
        catch (Exception e)
        {
            // ToDo: ???
        }
    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override Task<int> Receive(byte[] buffer)
    {
        return !Socket.Connected ? Task.FromResult(0) : Socket?.ReceiveAsync(buffer);
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(Memory<byte> buffer)
    {
        return !Socket.Connected ? await Task.FromResult(0) : await Socket.ReceiveAsync(buffer, SocketFlags.None);
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
        return !Socket.Connected ? Task.FromResult(0) : Socket.ReceiveAsync(buffer, offset, expectedBytesLength, SocketFlags.None);
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
        return !Socket.Connected ? Task.FromResult(0) : Socket.SendAsync(bytesToSend, offset, messageBytesLength);
    }

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public override bool Poll()
    {
        return Socket.Poll(PollingTimeout, SelectMode.SelectRead);
    }

    /// <summary>
    /// Send a file
    /// </summary>
    /// <param name="fileName">Full file path</param>
    public override void SendFile(string fileName)
    {
        Socket.SendFile(fileName);
    }

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public Socket Socket { get; protected set; }


    /// <summary>
    /// Prepare the answer of the socket for testing
    /// </summary>
    /// <param name="testData">Test data to use</param>
    public override void PrepareAnswer(byte[] testData)
    {
        // Do nothing
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        IsDisposed = true;
        Socket?.Close();
        Socket?.Dispose();
        Socket = null;
    }
}