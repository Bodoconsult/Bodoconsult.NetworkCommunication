// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Fake implementation of <see cref="ISocketProxy"/>
/// </summary>
public class FakeUdpSocketProxy : UpdSocketProxyBase
{
    /// <summary>
    /// Data for receiving methods
    /// </summary>
    private Memory<byte> _data = Array.Empty<byte>();

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
    /// Used to set return value for <see cref="Poll"/> method
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
    public override async ValueTask<int> Send(ReadOnlyMemory<byte> bytesToSend)
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
    /// Shut the socket down
    /// </summary>
    public override void Shutdown()
    {
        // Do nothing
    }

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
    /// Receive data from the socket (simulate TCP-IP implementation behaviour)
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(byte[] buffer)
    {
        if (ReceiverThrowSocketException)
        {
            throw new SocketException(999);
        }

        var i = await Task.Run(() =>
        {


            var length = buffer.Length;
            // Copy only data with length of buffer (like the TCP-IP implementation)
            if (_data.Length >= buffer.Length)
            {
                Buffer.BlockCopy(_data.ToArray(), 0, buffer, 0, buffer.Length);
            }
            else
            {
                length = _data.Length;
                Buffer.BlockCopy(_data.ToArray(), 0, buffer, 0, _data.Length);
            }

            _data = Array.Empty<byte>();
            return length;
        });

        return i;

    }

    /// <summary>
    /// Receive data from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <param name="offset">Offset</param>
    /// <param name="expectedBytesLength">Expected length of the byte data received</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength)
    {
        if (ReceiverThrowSocketException)
        {
            throw new SocketException(999);
        }

        var i = await Task.Run(() =>
        {
            Buffer.BlockCopy(_data.ToArray(), 0, buffer, offset, expectedBytesLength);
            return expectedBytesLength;
        });

        _data = Array.Empty<byte>();
        LoadNextReceivedMessage();
        return i;


    }

    /// <summary>
    /// Receive first data byte from the socket
    /// </summary>
    /// <param name="buffer">Byte array to store the received byte data in</param>
    /// <returns>Number of bytes received</returns>
    public override async Task<int> Receive(Memory<byte> buffer)
    {
        if (ReceiverThrowSocketException)
        {
            throw new SocketException(999);
        }

        var i = await Task.Run(() =>
        {
            _data.CopyTo(buffer);
            return _data.Length;
        });

        return i;
    }

    /// <summary>
    /// Send bytes 
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    /// <param name="offset">Offset</param>
    /// <param name="messageBytesLength">Number of message bytes length to send</param>
    /// <returns></returns>
    public override async Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
    {
        if (SenderThrowSocketException)
        {
            throw new SocketException(999);
        }

        // Do nothing
        var i = await Task.Run(() => DataLengthSent == int.MinValue ? bytesToSend.Length : DataLengthSent);
        _data = Array.Empty<byte>();
        return i;
    }

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public override bool Poll()
    {
        return IsPoll;
    }

    /// <summary>
    /// Send a file
    /// </summary>
    /// <param name="fileName">Full file path</param>
    public override void SendFile(string fileName)
    {
        throw new NotSupportedException();
    }


    /// <summary>
    /// Prepare the answer of the socket for testing
    /// </summary>
    /// <param name="testData">Test data to use</param>
    public override void PrepareAnswer(byte[] testData)
    {
        _data = new byte[testData.Length];
        testData.CopyTo(_data.ToArray(), 0);
    }
}