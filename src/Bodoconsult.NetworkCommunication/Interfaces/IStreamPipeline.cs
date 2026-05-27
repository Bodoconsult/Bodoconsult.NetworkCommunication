// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for stream based <see cref="IPipeline"/> implementations
/// </summary>
public interface IStreamPipeline : IPipeline
{
    ///// <summary>
    ///// Delegate to receive data from socket
    ///// </summary>
    //SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; set; }

    /// <summary>
    /// Current buffer
    /// </summary>
    public ReadOnlySequence<byte> Buffer { get; set; }

    /// <summary>
    /// Move the buffer position forward to fit with the new length of the buffer
    /// </summary>
    /// <param name="newLength">New length of the buffer after consuming data out of it</param>
    public void MoveForward(long newLength);
}