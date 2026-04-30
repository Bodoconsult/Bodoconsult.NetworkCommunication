// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using System.Diagnostics;
using System.Text;
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
    public void TryReadCommand_ValidDataMessage_CommandReturned()
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
        var data = new byte[] { 0x6, 0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c, 0x31, 0xa };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
            Assert.That(ros.Length, Is.EqualTo(27));
            Debug.Print(Encoding.UTF8.GetString(command));
        }

        Debug.Print(Encoding.UTF8.GetString(ros));

        // Act  
        var result2 = _splitter.TryReadCommand(ref ros, out var command2);

        Debug.Print(Encoding.UTF8.GetString(command2));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result2, Is.True);
            Assert.That(command2.Length, Is.EqualTo(27));
            
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
}