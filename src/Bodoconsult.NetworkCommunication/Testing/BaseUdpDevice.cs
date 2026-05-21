// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Base class for UDP client or server implementations
/// </summary>
public abstract class BaseUdpDevice : IUdpDevice
{
    /// <summary>
    /// Is the socket already disposed?
    /// </summary>
    protected bool IsDisposed;

    private readonly bool _isServer;

    /// <summary>
    /// Current type name
    /// </summary>
    protected string TypeName;

    /// <summary>
    /// Current local device listener
    /// </summary>
    protected UdpClient Listener;

    /// <summary>
    /// Endpoint for listening (receiving)
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Endpoint for sending messages (only unicasts)
    /// Holds the IP and port of the sender. It will be updated when a message is received in case of unicast messaging.
    /// </summary>
    protected IPEndPoint? SenderEndPoint = new(IPAddress.Any, 0);

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server listens on</param>
    /// <param name="isServer">Is this a server instance?</param>
    /// <param name="isMulticast">Is it a multicast instance?</param>
    protected BaseUdpDevice(IPAddress ipAddress, int port, bool isServer, bool isMulticast)
    {
        _isServer = isServer;

        if (isMulticast)
        {
            Listener = new UdpClient();
        }
        else
        {
            Listener = _isServer ? new UdpClient() : new UdpClient(port);
        }


        //Listener.ExclusiveAddressUse = false;
        //Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //Listener.Client.ReceiveTimeout = ReceiveTimeout;
        //Listener.Client.SendTimeout = SendTimeout;
        //Listener.Client.Blocking = false;

        IpAddress = ipAddress;
        Port = port;
        TypeName = GetType().Name;
    }

    /// <summary>
    /// Reply to a received message. Default: false
    /// </summary>
    public bool ReplyToReceivedMessage { get; set; }

    /// <summary>
    /// IP address of the server
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Port the current device listens on
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Current cancellation token
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    /// All received messages
    /// </summary>
    public ConcurrentBag<ReadOnlyMemory<byte>> ReceivedMessages { get; } = [];

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    public virtual void StartReceiverLoop()
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public virtual Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        throw new NotSupportedException("Override in derived classes");
    }
    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public virtual void Send(byte[] data)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        ReceivedMessages.Clear();
        IsDisposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        try
        {
            CancellationTokenSource.Cancel();
        }
        catch
        {
            // Do nothing
        }

        try
        {
            Listener.Client.Shutdown(SocketShutdown.Both);
            Listener.Close();
            Listener.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }
}