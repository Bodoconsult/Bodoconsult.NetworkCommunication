// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Protocols;

/// <summary>
/// Current implementation of <see cref="IDatagramPipeline"/> for UPD datagrams
/// </summary>
public class DatagramPipeline : IDatagramPipeline
{
    private readonly ProducerConsumerQueue2<QueueItem> _currentPipeline = new();
    private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

    /// <summary>
    /// Default ctor
    /// </summary>
    public DatagramPipeline()
    {
        _currentPipeline.ConsumerTaskDelegate = ConsumerTaskDelegate;
        _currentPipeline.StartConsumer();
    }

    /// <summary>
    /// Buffer size to use
    /// </summary>
    public int BufferSize { get; set; } = 0x10000;

    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    public SocketReceivedDataDelegate? SocketReceivedDataDelegate { get; private set; }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    public void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate)
    {
        SocketReceivedDataDelegate = socketReceivedDataDelegate;
    }

    private void ConsumerTaskDelegate(QueueItem item)
    {
        // Make a copy of the byte data here
        SocketReceivedDataDelegate?.Invoke(item.Data.Memory[..item.Length].ToArray());
        ReleaseBuffer(item.Data);
    }

    /// <summary>
    /// Stop the receiver loop
    /// </summary>
    public void StopReceiverLoop()
    {
        
    }

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"><see cref="AutoResetEvent"/> to wait until the loop has been started</param>
    /// <returns></returns>
    public Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get an entity from entity
    /// </summary>
    /// <returns></returns>
    public IMemoryOwner<byte> GetBuffer()
    {
        var result = _pool.Rent(BufferSize);
        return result;
    }

    /// <summary>
    /// Release the buffer
    /// </summary>
    /// <param name="data">Buffer entity</param>
    public void ReleaseBuffer(IMemoryOwner<byte> data)
    {
        // ToDo: release array to pool
        for(var i = 0; i < data.Memory.Length;  i++)
        {
            data.Memory.Span[i] = 0x0;
        }

        data.Dispose();
    }


    /// <summary>
    /// Add memory to the buffer
    /// </summary>
    /// <param name="data">Data to add to the buffer</param>
    /// <param name="length">Length of the data to add (may not be equal to the length of the data buffer!)</param>
    public void AddMemory(IMemoryOwner<byte> data, int length)
    {
        _currentPipeline.Enqueue(new QueueItem
        {
            Data = data,
            Length = length
        });
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _pool.Dispose();
        _currentPipeline.StopConsumer();
        _currentPipeline.Dispose();
    }

    /// <summary>
    /// Internal queue item
    /// </summary>
    private struct QueueItem
    {
        /// <summary>
        /// Memory
        /// </summary>
        public IMemoryOwner<byte> Data { get; init; }

        /// <summary>
        /// Length of the data stored in <see cref="Data"/>
        /// </summary>
        public int Length { get; init; }
    }
}