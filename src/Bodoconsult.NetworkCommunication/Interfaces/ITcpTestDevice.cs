// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for TCP/IP devices for testing
/// </summary>
public interface ITcpIpDevice: IDisposable
{
    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    int SendTimeout { get; set; }

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    int ReceiveTimeout { get; set; }

    /// <summary>
    /// Start the server mode
    /// </summary>
    void Start();

    /// <summary>
    /// Reset the client socket if necessary
    /// </summary>
    void ResetClientSocket();

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    void Send(byte[] data);

    void Dispose(bool disposing);
}