// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Net;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class FakeTcpIpSocketProxyTests : BaseTestsTcpIpSocket
{
    /// <summary>
    /// Setup for each test
    /// </summary>
    [SetUp]
    public void TestSetup()
    {
        Socket = new FakeTcpIpSocketProxy();

        CurrentIpEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
    }


    [Test]
    public void Receive_MemoryLoadNextReceivedMessage_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };

        socket.AddReceivedMessage(receivedMessage);
        socket.LoadNextReceivedMessage();

        var data = new byte[receivedMessage.Length];
        var buffer = new Memory<byte>(data);

        // Act  
            
        var task = socket.Receive(buffer);
        task.Wait();

        // Assert
        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));

        buffer.Slice(0, 1).Span[0] = 0;
        buffer.Slice(1, 1).Span[0] = 0;
        buffer.Slice(2, 1).Span[0] = 1;
    }

    [Test]
    public void Receive_ArrayLoadNextReceivedMessage_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };

        socket.AddReceivedMessage(receivedMessage);
        socket.LoadNextReceivedMessage();

        var buffer = new byte[receivedMessage.Length];

        // Act  
            
        var task = socket.Receive(buffer);
        task.Wait(500);

        // Assert
        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));

        buffer[0] = 0;
        buffer[1] = 0;
        buffer[2] = 1;
    }

    [Test]
    public void Receive_MemorySend_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };

        socket.AddReceivedMessage(receivedMessage);
        socket.Send(receivedMessage).Wait();

        var data = new byte[receivedMessage.Length];
        var buffer = new Memory<byte>(data);

        // Act  
        var task = socket.Receive(buffer);
        task.Wait();

        // Assert
        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));

        Assert.That(buffer.Slice(0, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer.Slice(1, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer.Slice(2, 1).Span[0], Is.EqualTo(1));
    }

    [Test]
    public void Receive_MemorySend2Messages_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };
        var receivedMessage2 = new byte[] { 0, 0, 0, 2 };

        socket.AddReceivedMessage(receivedMessage);
        socket.AddReceivedMessage(receivedMessage2);
        socket.Send(receivedMessage).Wait();

        var data = new byte[receivedMessage.Length];
        var buffer = new Memory<byte>(data);
        var data2 = new byte[receivedMessage2.Length];
        var buffer2 = new Memory<byte>(data2);

        // Act  
        var task = socket.Receive(buffer);
        task.Wait();

        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));

        socket.Send(receivedMessage).Wait();
            
        task = socket.Receive(buffer2);
        task.Wait();

        Assert.That(task.Result, Is.EqualTo(receivedMessage2.Length));


        // Assert
        Assert.That(buffer.Slice(0, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer.Slice(1, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer.Slice(2, 1).Span[0], Is.EqualTo(1));

        Assert.That(buffer2.Slice(0, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer2.Slice(1, 1).Span[0], Is.EqualTo(0));
        Assert.That(buffer2.Slice(2, 1).Span[0] , Is.EqualTo(0));
        Assert.That(buffer2.Slice(3, 1).Span[0] , Is.EqualTo(2));
    }

    [Test]
    public void Receive_ArraySends_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };

        socket.AddReceivedMessage(receivedMessage);
        socket.Send(receivedMessage).Wait();

        var buffer = new byte[receivedMessage.Length];

        // Act  
        var task = socket.Receive(buffer);
        task.Wait();

            

        // Assert
        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));
        Assert.That(buffer[0], Is.EqualTo(0));
        Assert.That(buffer[1], Is.EqualTo(0));
        Assert.That(buffer[2], Is.EqualTo(1));
    }

    [Test]
    public void Receive_ArraySend2Messages_DataReceived()
    {
        // Arrange 
        var socket = (FakeTcpIpSocketProxy)Socket;

        var receivedMessage = new byte[] { 0, 0, 1 };
        var receivedMessage2 = new byte[] { 0, 0, 0, 2 };

        socket.AddReceivedMessage(receivedMessage);
        socket.AddReceivedMessage(receivedMessage2);
        socket.Send(receivedMessage).Wait();

        var buffer = new byte[receivedMessage.Length];
        var buffer2 = new byte[receivedMessage2.Length];


        // Act  
        var task = socket.Receive(buffer);
        task.Wait();

        Assert.That(task.Result, Is.EqualTo(receivedMessage.Length));

        socket.Send(receivedMessage).Wait();
            
        task = socket.Receive(buffer2);
        task.Wait();

        Assert.That(task.Result, Is.EqualTo(receivedMessage2.Length));

        // Assert
        Assert.That(buffer[0], Is.EqualTo(0));
        Assert.That(buffer[1], Is.EqualTo(0));
        Assert.That(buffer[2] , Is.EqualTo(1));

        Assert.That(buffer2[0], Is.EqualTo(0));
        Assert.That(buffer2[1], Is.EqualTo(0));
        Assert.That(buffer2[2], Is.EqualTo(0));
        Assert.That(buffer2[3], Is.EqualTo(2));
    }

}