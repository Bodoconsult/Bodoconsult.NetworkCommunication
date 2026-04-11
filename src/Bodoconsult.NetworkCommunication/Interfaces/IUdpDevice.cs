// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for UDP devices
/// </summary>
public interface IUdpDevice: IDisposable
{
    /// <summary>
    /// IP address of the server
    /// </summary>
    IPAddress IpAddress { get; }

    /// <summary>
    /// Port the current device listens on
    /// </summary>
    int Port { get; }

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    int SendTimeout { get; set; }

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    int ReceiveTimeout { get; set; }

    /// <summary>
    /// Current cancellation token
    /// </summary>
    CancellationTokenSource CancellationTokenSource { get; set; }

    /// <summary>
    /// All received messages
    /// </summary>
    ConcurrentBag<ReadOnlyMemory<byte>> ReceivedMessages { get; }

    /// <summary>
    /// Start the client
    /// </summary>
    void Start();

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    void Send(byte[] data);

    /// <summary>
    /// Receive data
    /// </summary>
    /// <returns>Received data</returns>
    Task<byte[]> Receive();
}