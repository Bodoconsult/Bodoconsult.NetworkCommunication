// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Base class for TCP/IP devices
/// </summary>
public abstract class BaseTcpIpDevice : ITcpIpDevice
{
    /// <summary>
    /// Current socket
    /// </summary>
    protected Socket? Socket;

    /// <summary>
    /// Is this a server instance?
    /// </summary>
    protected bool IsServer;

    /// <summary>
    /// Current logger ID
    /// </summary>
    protected string LoggerId =string.Empty;

    /// <summary>
    /// Maximum buffer size for TCP packages. Set this value lower if your packages do not reach the maximum length of 65536 byte for UDP diagrams defined by protocol specs
    /// </summary>
    public int MaxPacketSize { get; set; } = 65536;

    /// <summary>
    /// All received messages
    /// </summary>
    public ConcurrentBag<ReadOnlyMemory<byte>> ReceivedMessages { get; } = [];

    /// <summary>
    /// Current cancellation token
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Reply to a received message. Default: false
    /// </summary>
    public bool ReplyToReceivedMessage { get; set; }

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
    /// Reset the client socket if necessary
    /// </summary>
    public virtual void ResetClientSocket()
    {
        if (Socket == null)
        {
            return;
        }
        Socket.Close();
        Socket = null;
    }

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public virtual void Send(byte[] data)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Receive data
    /// </summary>
    /// <returns>Received data</returns>
    public virtual Task<byte[]> Receive()
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Dispose the instance
    /// </summary>
    /// <param name="disposing">Is disposing?</param>
    public virtual void Dispose(bool disposing)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}