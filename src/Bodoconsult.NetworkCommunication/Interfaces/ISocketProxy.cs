// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for IP socket implementations
/// </summary>
public interface ISocketProxy: IDisposable
{


    /// <summary>
    /// Logger ID or null
    /// </summary>
    string? LoggerId { get; set; }

    /// <summary>
    /// Minimum buffer size
    /// </summary>
    int MinimumBufferSize { get; set; } 

    /// <summary>
    /// IP address of the remote device
    /// </summary>
    IPAddress? IpAddress { get; set; }

    /// <summary>
    /// Port the current device listens on
    /// </summary>
    int Port { get; set; }

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
    /// Current cancellation token
    /// </summary>
    CancellationTokenSource CancellationTokenSource { get; set; }

    /// <summary>
    /// Current logger to use or null. This logger logs only exceptions but NO data due to potential performance issues
    /// </summary>
    IAppLoggerProxy Logger { get; }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    Task<int> Send(byte[] bytesToSend);

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    Task<int> Send(ReadOnlyMemory<byte> bytesToSend);

    /// <summary>
    /// Close the socket
    /// </summary>
    void Close();

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    Task Connect();

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    bool Poll();
}