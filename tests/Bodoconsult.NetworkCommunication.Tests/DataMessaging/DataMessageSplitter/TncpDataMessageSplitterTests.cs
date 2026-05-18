// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using System.Diagnostics;
using System.Text;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageSplitter;

[TestFixture]
internal class TncpDataMessageSplitterTests
{
    private readonly TncpDataMessageSplitter _splitter = new();

    [Test]
    public void TryReadCommand_NoValidMessage_NullReturned()
    {
        // Arrange 
        var data = new byte[] { 0x99, 0x1, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out _);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryReadCommand_ValidDataRequestMessage_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x1, 0x99, 0x99, DeviceCommunicationBasics.Lf, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(4));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataReplyMessage_CommandReturned()
    {
        // Arrange 
        const string reply = "<BEGIN>show,streamconfig\n<CONFIG>0x00x10x20x30xC0xF\n<END>\n";

        var data = Encoding.UTF8.GetBytes(reply);
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        Debug.Print(Encoding.UTF8.GetString(command));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(reply.Length));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataReplyMessage2_CommandReturned()
    {
        // Arrange 
        const string reply = "<BEGIN>set,stream,order,1,2,3,4\n<END>\n";

        var data = Encoding.UTF8.GetBytes(reply);
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        Debug.Print(Encoding.UTF8.GetString(command));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(reply.Length));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataReplyMessage3_CommandReturned()
    {
        // Arrange 
        const string reply = "<BEGIN>set,stream,order,1,2,3,4\n<END>\n";

        var data = new byte[] {  0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6f, 0x72, 0x64, 0x65, 0x72, 0x2c, 0x31, 0x2c, 0x32, 0x2c, 0x33, 0x2c, 0x34, 0xa, 0x3c, 0x45, 0x4e, 0x44, 0x3e, 0xa };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        Debug.Print(Encoding.UTF8.GetString(command));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(reply.Length));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataMessage2_CommandReturned()
    {
        // Arrange 
        var data = new byte[]
        {
            0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c,
            0x31, DeviceCommunicationBasics.Lf, 0x99
        };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(data.Length - 1));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataMessage3_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c, 0x34, DeviceCommunicationBasics.Lf, 
            0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6d, 0x6f, 0x64, 0x65, 0x2c, 0x73, 0x6e, 
            0x61, 0x70, 0x73, 0x68, 0x6f, 0x74, 0x2c, 0x63, 0x6f, 0x6e, 0x74, 0x69, 0x6e, 0x69, 0x6f, 0x75, 0x73, DeviceCommunicationBasics.Lf };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(data.Length - ros.Length));
            Debug.Print(Encoding.UTF8.GetString(command));
        }

        // Act  
        var result2 = _splitter.TryReadCommand(ref ros, out var command2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result2, Is.True);
            Assert.That(command2.Length, Is.EqualTo(data.Length - command.Length- ros.Length));
            Debug.Print(Encoding.UTF8.GetString(command2));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataMessage4_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x6, 0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c, 0x31, 0xa, 0x3c, 0x45, 0x4e, 0x44, 0x3e };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
            Assert.That(ros.Length, Is.EqualTo(32));
            Debug.Print(Encoding.UTF8.GetString(command));
        }

        Debug.Print(Encoding.UTF8.GetString(ros));

        // Act  
        var result2 = _splitter.TryReadCommand(ref ros, out var command2);

        Debug.Print(Encoding.UTF8.GetString(command2));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result2, Is.False);
            Assert.That(command2.Length, Is.Zero);
            
        }
    }


    [Test]
    public void TryReadCommand_ValidHandshakeAck_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x99, 0x99, DeviceCommunicationBasics.Ack, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
        }
    }

    [Test]
    public void TryReadCommand_ValidHandshakeNack_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x99, 0x99, DeviceCommunicationBasics.Nack, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
        }
    }

    [Test]
    public void TryReadCommand_ValidHandshakeCan_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { 0x99, 0x99, DeviceCommunicationBasics.Can, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
        }
    }


    [Test]
    public void CheckIfReply_Reply_ReturnsTrue()
    {
        // Arrange 
        const string cmd = "<BEGIN>show,streamconfig";

        var rs = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(cmd));
        
        // Act  
        var result = TncpDataMessageSplitter.CheckIfReply(rs, 0);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckIfReply_Request_ReturnsFalse()
    {
        // Arrange 
        const string cmd = "show,streamconfig";

        var rs = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(cmd));

        // Act  
        var result = TncpDataMessageSplitter.CheckIfReply(rs, 0);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckIfRequest_InvalidEnd_ReturnsFalse()
    {
        // Arrange 
        const string cmd = "Blu<END>\n";

        var rs = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(cmd));

        // Act  
        var result = TncpDataMessageSplitter.CheckIfEnd(rs, 4);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CheckIfRequest_InvalidEnd_ReturnsTrue()
    {
        // Arrange 
        const string cmd = "Blu<END>\n";

        var rs = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(cmd));

        // Act  
        var result = TncpDataMessageSplitter.CheckIfEnd(rs, 8);

        // Assert
        Assert.That(result, Is.True);
    }

}