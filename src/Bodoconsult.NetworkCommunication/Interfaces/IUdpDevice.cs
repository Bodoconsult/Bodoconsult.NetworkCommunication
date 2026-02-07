// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

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
    /// Port the remote device listens on or 0 (then the same port as for the current device is used)
    /// </summary>
    int RemotePort { get; }

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
    List<ReadOnlyMemory<byte>> ReceivedMessages { get; }

    /// <summary>
    /// Start the client
    /// </summary>
    void Start();

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    void Send(byte[] data);
}