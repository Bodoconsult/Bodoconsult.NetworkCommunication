// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols;

/// <summary>
/// Current implementation of <see cref="IDatagramPipeline"/> for UPD datagrams
/// </summary>
public class DatagramPipeline : IDatagramPipeline
{
    private readonly ProducerConsumerQueue<byte[]> _currentPipeline = new()
    {
        ThreadPriority = ThreadPriority.AboveNormal
    };

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

    private void ConsumerTaskDelegate(byte[] item)
    {
        // Make a copy of the byte data here
        //var ms = item.Buffer.AsMemory()[..item.Buffer.Length].ToArray();
        //SocketReceivedDataDelegate?.Invoke(item.Buffer.AsMemory()[..item.Buffer.Length]);

        //Debug.Print($"Pipeline: {ArrayHelper.GetStringFromArrayCsharpStyle(item.AsMemory().Slice(0,8 ), false)}");

        SocketReceivedDataDelegate?.Invoke(item.AsMemory());
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
    public void AddMemory(byte[] data)
    {
        if (data.Length == 0)
        {
            return;
        }

        _currentPipeline.Enqueue(data);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _currentPipeline.StopConsumer();
        _currentPipeline.Dispose();
    }
}