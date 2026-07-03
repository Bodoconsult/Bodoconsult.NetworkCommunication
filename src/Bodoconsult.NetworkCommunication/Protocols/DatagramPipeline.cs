// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Protocols;

/// <summary>
/// Current implementation of <see cref="IDatagramPipeline"/> for UPD datagrams
/// </summary>
public class DatagramPipeline : IDatagramPipeline
{
    private readonly ProducerConsumerQueue2<Memory<byte>> _currentPipeline = new()
    {
        ThreadPriority = ThreadPriority.Highest
    };

    /// <summary>
    /// Inbound queue for received UDP result
    /// </summary>
    public ProducerConsumerQueue<byte[]> InboundQueue { get; } = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    public DatagramPipeline()
    {
        _currentPipeline.ConsumerTaskDelegate = ConsumerTaskDelegate;
        _currentPipeline.StartConsumer();

        InboundQueue.ConsumerTaskDelegate = ConsumerTaskDelegate2;
        InboundQueue.StartConsumer();
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

    private void ConsumerTaskDelegate(Memory<byte> item)
    {
        if (item.Length == 0)
        {
            return;
        }

        // Debug.Print($"Pipe: Q1 {_currentPipeline.InternalQueue.Count} Q2 {InboundQueue.InternalQueue.Count}");

        // Make a copy of the byte data here
        //var ms = item.Buffer.AsMemory()[..item.Buffer.Length].ToArray();
        //SocketReceivedDataDelegate?.Invoke(item.Buffer.AsMemory()[..item.Buffer.Length]);

        //Debug.Print($"Pipeline: {ArrayHelper.GetStringFromArrayCsharpStyle(item.AsMemory().Slice(0,8 ), false)}");

        SocketReceivedDataDelegate?.Invoke(item);
    }


    private void ConsumerTaskDelegate2(byte[] data)
    {
        if (data.Length == 0)
        {
            return;
        }

        //Debug.Print($"DP {data[0]}");

        AddMemory(data);
    }

    /// <summary>
    /// Stop the receiver loop
    /// </summary>
    public void StopReceiverLoop()
    { }

    /// <summary>
    /// Add the received data to the queue
    /// </summary>
    /// <param name="data">Received data</param>
    public void AddMemory(Memory<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        _currentPipeline.Enqueue(data);
    }

    /// <summary>
    /// Add a received UDP result to the inbound queue
    /// </summary>
    /// <param name="result">Received UDP result</param>
    public void AddResult(byte[] result)
    {
        InboundQueue.Enqueue(result);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _currentPipeline.StopConsumer();
        _currentPipeline.Dispose();

        InboundQueue.StopConsumer();
        InboundQueue.Dispose();
    }
}