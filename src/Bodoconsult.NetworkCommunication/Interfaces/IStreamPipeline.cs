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
    /// Get a buffer
    /// </summary>
    /// <returns>Buffer entity</returns>
    IMemoryOwner<byte> GetBuffer();

    /// <summary>
    /// Release the buffer
    /// </summary>
    /// <param name="data">Buffer entity</param>
    void ReleaseBuffer(IMemoryOwner<byte> data);

    /// <summary>
    /// Add memory to the buffer
    /// </summary>
    /// <param name="data">Data to add to the buffer</param>
    /// <param name="length">Length of the data to add (may not be equal to the length of the data buffer!)</param>
    public void AddMemory(IMemoryOwner<byte> data, int length);

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