// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Net;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for TCP/IP socket implementations
/// </summary>
public interface ISocketProxy: IDisposable
{
    /// <summary>
    /// Is the instance already dispossed
    /// </summary>
    bool IsDisposed  { get;  }

    /// <summary>
    /// Timeout for polling in milliseconds
    /// </summary>
    int PollingTimeout { get; set; }

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    int BytesAvailable { get; }


    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    int SendTimeout { get; set; }

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    int ReceiveTimeout { get; set; }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    bool Connected { get;  }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    Task<int> Send(byte[] bytesToSend);

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend);

    /// <summary>
    /// Shut the socket down
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Close the socket
    /// </summary>
    void Close();

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    /// <param name="endpoint">IP endpoint</param>
    Task Connect(IPEndPoint endpoint);

    /// <summary>
    /// Bind to an IP endpoint
    /// </summary>
    /// <param name="endpoint">IP endpoint</param>
    void Bind(IPEndPoint endpoint);


    /// <summary>
    /// Listen
    /// </summary>
    /// <param name="backlog">The maximum length of pending messages queue</param>
    void Listen(int backlog);


    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    Task<int> Receive(byte[] buffer);

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    Task<int> Receive(Memory<byte> buffer);

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <param name="offset">Offset</param>
    /// <param name="expectedBytesLength">Expected length of the byte data received</param>
    /// <returns>Number of bytes received</returns>
    Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength);

    /// <summary>
    /// Send bytes 
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    /// <param name="offset">Offset</param>
    /// <param name="messageBytesLength">Number of message bytes length to send</param>
    /// <returns></returns>
    Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength);

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    bool Poll();

    /// <summary>
    /// Send a file
    /// </summary>
    /// <param name="fileName">Full file path</param>
    void SendFile(string fileName);

    /// <summary>
    /// Prepare the answer of the socket for testing
    /// </summary>
    /// <param name="testData">Test data to use</param>
    void PrepareAnswer(byte[] testData);
}