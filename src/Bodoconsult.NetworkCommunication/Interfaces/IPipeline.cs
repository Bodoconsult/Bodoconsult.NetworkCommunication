// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for comm data pipelines
/// </summary>
public interface IPipeline : IDisposable
{
    /// <summary>
    /// Buffer size to use
    /// </summary>
    int BufferSize { get; set; }

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
}