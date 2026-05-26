// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BufferPool;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Delegates;
using System.IO.Pipelines;

namespace Bodoconsult.NetworkCommunication.Protocols
{
    /// <summary>
    /// Current implementation of <see cref="IStreamPipeline"/> for TCP/IP
    /// </summary>
    public class StreamPipeline : IStreamPipeline
    {
        private readonly Pipe _pipe = new();
        private bool _isStarted;

        /// <summary>
        /// Buffer size to use
        /// </summary>
        public int BufferSize { get; set; } = 512;

        /// <summary>
        /// Delegate to receive data from socket
        /// </summary>
        public SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; set; }

        /// <summary>
        /// Get a buffer
        /// </summary>
        /// <returns>Buffer entity</returns>
        public Memory<byte> GetBuffer()
        {
            return _pipe.Writer.GetMemory(BufferSize);
        }

        /// <summary>
        /// Write data to the pipeline
        /// </summary>
        /// <param name="length">Length of the received data</param>
        /// <returns></returns>
        public async Task WriteToPipeline(int length)
        {
            // Tell the PipeWriter how much was read from the Socket.
            _pipe.Writer.Advance(length);

            // Now flush the writer to forward the data to the reader
            var result = await _pipe.Writer.FlushAsync();

            if (result is { IsCompleted: false, IsCanceled: false } && !_isStarted)
            {
                _isStarted = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ReadBuffer()
        {
            if (!_isStarted)
            {
                return false;
            }

            var reader = _pipe.Reader;

            try
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;

                if (buffer.IsEmpty)
                {
                    return result.IsCompleted;
                }

                ////Trace.TraceInformation($"Raw command: {ArrayHelper.GetStringFromArrayCsharpStyle(ref buffer)}");
                //MonitorLogger?.LogInformation(
                //    $"Raw command: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}");

                SocketReceivedDataDelegate?.Invoke(ref buffer);

                reader.AdvanceTo(buffer.Start, buffer.End);

                return result.IsCompleted;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _isStarted = false;
            _pipe.Writer.Complete();
            // Do nothing
        }
    }

    /// <summary>
    /// Interface for stream based <see cref="IPipeline"/> implementations
    /// </summary>
    public interface IStreamPipeline : IPipeline
    {
        /// <summary>
        /// Delegate to receive data from socket
        /// </summary>
        SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; set; }

        /// <summary>
        /// Get a buffer
        /// </summary>
        /// <returns>Buffer entity</returns>
        Memory<byte> GetBuffer();

        /// <summary>
        /// Write data to the pipeline
        /// </summary>
        /// <param name="length">Length of the received data</param>
        /// <returns></returns>
        Task WriteToPipeline(int length);

        /// <summary>
        /// Read the buffer
        /// </summary>
        /// <returns>Read data sequence</returns>
        Task<bool> ReadBuffer();
    }

    /// <summary>
    /// Base interface for comm data pipelines
    /// </summary>
    public interface IPipeline : IDisposable
    {
        /// <summary>
        /// Buffer size to use
        /// </summary>
        int BufferSize { get; set; }


    }

    /// <summary>
    /// Interface for stream based <see cref="IPipeline"/> implementations
    /// </summary>
    public interface IDatagramPipeline : IPipeline
    {
        /// <summary>
        /// Delegate fired if the socket was receiving data
        /// </summary>
        SocketReceivedDataDelegate? SocketReceivedDataDelegate { get; set; }

        /// <summary>
        /// Start the receiver loop
        /// </summary>
        /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
        void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate);

        /// <summary>
        /// Stop the receiver loop
        /// </summary>
        void StopReceiverLoop();

        /// <summary>
        /// Run the receiver loop
        /// </summary>
        /// <param name="waitForLoopStarted"><see cref="AutoResetEvent"/> to wait until the loop has been started</param>
        /// <returns></returns>
        Task ReceiverLoop(AutoResetEvent waitForLoopStarted);

        /// <summary>
        /// Get an entity from entity
        /// </summary>
        /// <returns></returns>
        DummyMemory GetBuffer();

        /// <summary>
        /// Release the buffer
        /// </summary>
        /// <param name="data">Buffer entity</param>
        void ReleaseBuffer(DummyMemory data);
    }


    /// <summary>
    /// Current implementation of <see cref="IDatagramPipeline"/> for UPD datagrams
    /// </summary>
    public class DatagramPipline : IDatagramPipeline
    {
        private readonly BufferPool<DummyMemory> _bufferPool = new();
        private readonly ProducerConsumerQueue<DummyMemory> _currentPipeline = new();

        /// <summary>
        /// Default ctor
        /// </summary>
        public DatagramPipline()
        {
            _bufferPool.LoadFactoryMethod(() => new DummyMemory());
            _bufferPool.Allocate(3);
            
        }

        /// <summary>
        /// Buffer size to use
        /// </summary>
        public int BufferSize { get; set; } = 0x10000;

        /// <summary>
        /// Delegate fired if the socket was receiving data
        /// </summary>
        public SocketReceivedDataDelegate? SocketReceivedDataDelegate  { get; set; }

        /// <summary>
        /// Start the receiver loop
        /// </summary>
        /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
        public void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate)
        {
            _currentPipeline.ConsumerTaskDelegate = ConsumerTaskDelegate;
            _currentPipeline.StartConsumer();
        }

        private void ConsumerTaskDelegate(DummyMemory data)
        {
            SocketReceivedDataDelegate?.Invoke(data.Memory.ToArray());
            ReleaseBuffer(data);
        }

        /// <summary>
        /// Stop the receiver loop
        /// </summary>
        public void StopReceiverLoop()
        {
            throw new NotImplementedException();
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
        public DummyMemory GetBuffer()
        {
            var result = _bufferPool.Dequeue();

            // ToDo: use array pool
            result.Memory = new Memory<byte>(new byte[BufferSize]);

            return result;
        }

        /// <summary>
        /// Release the buffer
        /// </summary>
        /// <param name="data">Buffer entity</param>
        public void ReleaseBuffer(DummyMemory data)
        {
            // ToDo: release array to pool
            data.Memory = Memory<byte>.Empty;
            _bufferPool.Enqueue(data);
        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _currentPipeline.StopConsumer();
            _currentPipeline.Dispose();
        }
    }
}
