// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Fake implementation of <see cref="ISocketProxy"/>
/// </summary>
public class FakeTcpIpSocketProxy : BaseTcpIpSocketProxy
{
    /// <summary>
    /// Data for receiving methods
    /// </summary>
    private Memory<byte> _data = Array.Empty<byte>();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public FakeTcpIpSocketProxy(IAppLoggerProxy logger) : base(logger, new StreamPipeline())
    { }

    /// <summary>
    /// Bytes available on the socket
    /// </summary>
    public override int BytesAvailable => _data.Length;

    /// <summary>
    /// Length of data sent. If set to default value
    /// int.MinValue the array length is returned else the provide length
    /// </summary>
    public int DataLengthSent { get; set; } = int.MinValue;

    /// <summary>
    /// Queue for received messages
    /// </summary>
    public Queue<Memory<byte>> ReceivedMessages { get; set; } = new();

    /// <summary>
    /// Add a message to the <see cref="ReceivedMessages"/> queue
    /// </summary>
    /// <param name="receivedMessage">Received message to store</param>
    public void AddReceivedMessage(byte[] receivedMessage)
    {
        ReceivedMessages.Enqueue(receivedMessage);
    }

    /// <summary>
    /// Add a message to the <see cref="ReceivedMessages"/> queue
    /// </summary>
    /// <param name="receivedMessage">Received message to store</param>
    public void AddReceivedMessage(Memory<byte> receivedMessage)
    {
        ReceivedMessages.Enqueue(receivedMessage);
    }

    /// <summary>
    /// Load the next message from <see cref="ReceivedMessages"/>
    /// </summary>
    public void LoadNextReceivedMessage()
    {
        if (ReceivedMessages.Count == 0)
        {
            _data = Array.Empty<byte>();
            return;
        }
        var success = ReceivedMessages.TryDequeue(out var data);

        if (success)
        {
            _data = data;
        }
    }

    /// <summary>
    /// Used to set value for <see cref="Connected"/> prop
    /// </summary>
    public bool IsConnected { get; set; } = true;

    /// <summary>
    /// Used to set return value for Poll method
    /// </summary>
    public bool IsPoll { get; set; }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected => IsConnected;

    /// <summary>
    /// Throw a socket exception during receiving
    /// </summary>
    public bool ReceiverThrowSocketException { get; set; }

    /// <summary>
    /// Throw a socket exception during sending
    /// </summary>
    public bool SenderThrowSocketException { get; set; }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override async Task<int> Send(byte[] bytesToSend)
    {
        if (SenderThrowSocketException)
        {
            throw new SocketException(999);
        }

        var i = await Task.Run(() =>
        {
            LoadNextReceivedMessage();
            return DataLengthSent == int.MinValue ? bytesToSend.Length : DataLengthSent;
        });
            
        return i;
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override async Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        if (SenderThrowSocketException)
        {
            throw new SocketException(999);
        }

        var i = await Task.Run(() =>
        {
            LoadNextReceivedMessage();
            return DataLengthSent == int.MinValue ? bytesToSend.Length : DataLengthSent;
        });
        return i;
    }

    ///// <summary>
    ///// Shut the socket down
    ///// </summary>
    //public override void Shutdown()
    //{
    //    // Do nothing
    //}

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        // Do nothing
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {
        await Task.Run(() =>
        {
            // Do nothing
        });
    }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    public override void StartReceiverLoop(SocketReceivedDataDelegate2 socketReceivedDataDelegate)
    {
        SocketReceivedDataDelegate = socketReceivedDataDelegate;
    }

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public override Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        if (SenderThrowSocketException)
        {
            throw new SocketException(999);
        }

        SocketReceivedDataDelegate?.Invoke();
        return Task.CompletedTask;
    }
}