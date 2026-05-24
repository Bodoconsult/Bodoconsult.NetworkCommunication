// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Base class for <see cref="ISocketProxy"/> implementations
/// </summary>
public abstract class BaseUpdSocketProxy : IUdpSocketProxy
{
    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    protected UdpClient? UdpClient;

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseUpdSocketProxy(IAppLoggerProxy logger)
    {
        LoggerId = $"{GetType().Name}: ";
        Logger = logger;
        Pipeline = new DatagramPipline();
    }

    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    public SocketReceivedDataDelegate? SocketReceivedDataDelegate { get; protected set; }

    /// <summary>
    /// Logger ID or null
    /// </summary>
    public string? LoggerId { get; set; }

    /// <summary>
    /// Minimum buffer size
    /// </summary>
    public int MinimumBufferSize { get; set; } = 0x10000;

    /// <summary>
    /// IP address of the server
    /// </summary>
    public IPAddress? IpAddress { get; set; }

    /// <summary>
    /// Port the current device listens on
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Is the instance already dispossed
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Timeout for polling in milliseconds
    /// </summary>
    public int PollingTimeout { get; set; } = 1000;

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    public virtual int BytesAvailable => 0;


    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public virtual bool Connected { get; } = true;

    /// <summary>
    /// Current cancellation token
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    /// Current logger to use or null. This logger logs only exceptions but NO data due to potential performance issues
    /// </summary>
    public IAppLoggerProxy Logger { get; }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    public virtual void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate)
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
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public virtual Task<int> Send(byte[] bytesToSend)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public virtual Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public virtual void Close()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public virtual Task Connect()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public virtual bool Poll()
    {
        throw new NotSupportedException();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
        // Do nothing
        IsDisposed = true;
    }

    /// <summary>
    /// Datagrampipeline
    /// </summary>
    public IDatagramPipeline Pipeline { get; }
}