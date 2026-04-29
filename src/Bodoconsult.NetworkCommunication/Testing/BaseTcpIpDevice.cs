// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Base class for TCP/IP devices
/// </summary>
public abstract class BaseTcpIpDevice : ITcpIpDevice
{
    private Task? _thread;

    /// <summary>
    /// Current socket
    /// </summary>
    protected Socket? Socket;

    /// <summary>
    /// Is this a server instance?
    /// </summary>
    protected bool IsServer;

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
    /// Start the client
    /// </summary>
    public void Start()
    {
        _thread = Task.Run(WaitForMessages);
        Task.Delay(100);
    }

    private async Task WaitForMessages()
    {
        while (!CancellationTokenSource.Token.IsCancellationRequested)
        {
            //if (CancellationTokenSource.Token.IsCancellationRequested)
            //{
            //    return;
            //}

            //try
            //{
            var bytes = await Receive();

            if (!IsServer && ReplyToReceivedMessage)
            {
                Send(bytes);
            }
            //catch (Exception e)
            //{
            //    Trace.TraceInformation(e.ToString());
            //}
        }
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
        try
        {
            _thread?.Dispose();
        }
        catch
        {
            // Do nothing
        }

        Dispose(true);
        GC.SuppressFinalize(this);
    }
}