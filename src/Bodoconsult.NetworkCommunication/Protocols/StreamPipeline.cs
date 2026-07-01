// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Protocols;

/// <summary>
/// <see cref="IStreamPipeline"/> based pipeline implementation for streams
/// </summary>
public class StreamPipeline : IStreamPipeline
{
    private ReadOnlySequence<byte> _buffer = new([]);
    private readonly Lock _bufferLock = new();
    private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;
    private readonly ProducerConsumerQueue2<Memory<byte>> _queue = new();
    private readonly ProducerConsumerQueue<IMemoryOwner<byte>> _bufferQueue= new ();

    /// <summary>
    /// Default ctor
    /// </summary>
    public StreamPipeline()
    {
        _queue.ConsumerTaskDelegate = ConsumerTaskDelegate;
        _queue.StartConsumer();

        _bufferQueue.ConsumerTaskDelegate = ConsumerTaskDelegate2;
    }

    private void ConsumerTaskDelegate2(IMemoryOwner<byte> data)
    {
        ReleaseBuffer(data);
    }


    private void ConsumerTaskDelegate(Memory<byte> data)
    {
        lock (_bufferLock)
        {
            var chunk = new ChunkedSequence<byte>(_buffer);
            chunk.Append(data);
            _buffer = chunk;
        }

        SocketReceivedDataDelegate?.Invoke();
    }


    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    public SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; set; }

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
        for (var i = 0; i < data.Memory.Length; i++)
        {
            data.Memory.Span[i] = 0x0;
        }

        data.Dispose();
    }

    /// <summary>
    /// Add memory to the buffer
    /// </summary>
    /// <param name="data">Data to add to the buffer</param>
    /// <param name="dataLength"></param>
    public void AddMemory(IMemoryOwner<byte> data, int dataLength)
    {
        _queue.Enqueue(data.Memory[..dataLength].ToArray());
        _bufferQueue.Enqueue(data);
    }

    /// <summary>
    /// Current buffer
    /// </summary>
    public ReadOnlySequence<byte> Buffer
    {
        get
        {
            lock (_bufferLock)
            {
                return _buffer;
            }
        }
        set
        {
            lock (_bufferLock)
            {
                _buffer = value;
            }
        }
    }

    /// <summary>
    /// Move the buffer position forward to fit with the new length of the buffer
    /// </summary>
    /// <param name="newLength">New length of the buffer after consuming data out of it</param>
    public void MoveForward(long newLength)
    {
        lock (_bufferLock)
        {
            _buffer = _buffer.Slice(Buffer.Length - newLength);
        }
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _queue.StopConsumer();
        _bufferQueue.StopConsumer();
    }

    /// <summary>
    /// Buffer size to use
    /// </summary>
    public int BufferSize { get; set; } = 1024;

}

///// <summary>
///// Current implementation of <see cref="IStreamPipeline"/> for TCP/IP
///// </summary>
//public class StreamPipeline : IStreamPipeline
//{
//    private readonly Pipe _pipe = new();
//    private bool _isStarted;

//    /// <summary>
//    /// Buffer size to use
//    /// </summary>
//    public int BufferSize { get; set; } = 512;

//    /// <summary>
//    /// Delegate to receive data from socket
//    /// </summary>
//    public SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; set; }

//    /// <summary>
//    /// Get a buffer
//    /// </summary>
//    /// <returns>Buffer entity</returns>
//    public Memory<byte> GetBuffer()
//    {
//        return _pipe.Writer.GetMemory(BufferSize);
//    }

//    /// <summary>
//    /// Write data to the pipeline
//    /// </summary>
//    /// <param name="length">Length of the received data</param>
//    /// <returns></returns>
//    public async Task WriteToPipeline(int length)
//    {
//        // Tell the PipeWriter how much was read from the Socket.
//        _pipe.Writer.Advance(length);

//        // Now flush the writer to forward the data to the reader
//        var result = await _pipe.Writer.FlushAsync();

//        if (result is { IsCompleted: false, IsCanceled: false } && !_isStarted)
//        {
//            _isStarted = true;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <returns></returns>
//    public async Task<bool> ReadBuffer()
//    {
//        if (!_isStarted)
//        {
//            return false;
//        }

//        var reader = _pipe.Reader;

//        try
//        {
//            var result = await reader.ReadAsync();
//            var buffer = result.Buffer;

//            if (buffer.IsEmpty)
//            {
//                return result.IsCompleted;
//            }

//            ////Trace.TraceInformation($"Raw command: {ArrayHelper.GetStringFromArrayCsharpStyle(ref buffer)}");
//            //MonitorLogger?.LogInformation(
//            //    $"Raw command: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}");

//            SocketReceivedDataDelegate?.Invoke(ref buffer);

//            reader.AdvanceTo(buffer.Start, buffer.End);

//            return result.IsCompleted;
//        }
//        catch (Exception e)
//        {
//            return false;
//        }
//    }

//    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
//    public void Dispose()
//    {
//        _isStarted = false;
//        _pipe.Writer.Complete();
//        // Do nothing
//    }
//}