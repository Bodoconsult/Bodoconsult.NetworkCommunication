// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols;

/// <summary>
/// Base class for <see cref="ISocketProxy"/> implementations
/// </summary>
public abstract class BaseSocketProxy : ISocketProxy
{
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
    public virtual int BytesAvailable { get; } = 0;


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
    public virtual bool Connected { get; } = false;

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public virtual Task<int> Send(byte[] bytesToSend)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public virtual ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Shut the socket down
    /// </summary>
    public virtual void Shutdown()
    {
        throw new NotSupportedException();
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
    /// <param name="endpoint">IP endpoint</param>
    public virtual Task Connect(IPEndPoint endpoint)
    {
        throw new NotSupportedException();
    }


    /// <summary>
    /// Bind to an IP endpoint
    /// </summary>
    /// <param name="ipEndPoint">IP endpoint</param>
    public virtual void Bind(IPEndPoint ipEndPoint)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Listen
    /// </summary>
    /// <param name="backlog">The maximum length of pending messages queue</param>
    public virtual void Listen(int backlog)
    {
        throw new NotSupportedException();
    }


    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public virtual Task<int> Receive(byte[] buffer)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public virtual Task<int> Receive(Memory<byte> buffer)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <param name="offset">Offset</param>
    /// <param name="expectedBytesLength">Expected length of the byte data received</param>
    /// <returns>Number of bytes received</returns>
    public virtual Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send bytes 
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    /// <param name="offset">Offset</param>
    /// <param name="messageBytesLength">Number of message bytes length to send</param>
    /// <returns></returns>
    public virtual Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
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

    /// <summary>
    /// Send a file
    /// </summary>
    /// <param name="fileName">Full file path</param>
    public virtual void SendFile(string fileName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Prepare the answer of the socket for testing
    /// </summary>
    /// <param name="testData">Test data to use</param>
    public virtual void PrepareAnswer(byte[] testData)
    {
        throw new NotSupportedException();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
        // Do nothing
        IsDisposed = true;
    }
}