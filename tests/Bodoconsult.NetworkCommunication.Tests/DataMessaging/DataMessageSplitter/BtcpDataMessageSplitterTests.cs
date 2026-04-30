// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageSplitter;

[TestFixture]
internal class BtcpDataMessageSplitterTests
{
    private readonly BtcpDataMessageSplitter _splitter = new();

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
        var data = new byte[] { DeviceCommunicationBasics.Stx, 0x1, 0x99, 0x99, DeviceCommunicationBasics.Etx, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(5));
        }
    }


    [Test]
    public void TryReadCommand_Valid2DataMessages_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { DeviceCommunicationBasics.Ack, DeviceCommunicationBasics.Stx, 0x1, 0x99, 0x99, DeviceCommunicationBasics.Etx, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(1));
        }

        // Act 2
        var result2 = _splitter.TryReadCommand(ref ros, out var command2);

        // Assert 2
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result2, Is.True);
            Assert.That(command2.Length, Is.EqualTo(5));
        }
    }

    [Test]
    public void TryReadCommand_ValidDataMessageRealWorld1_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { DeviceCommunicationBasics.Stx, 0x1, 0x99, 0x99, DeviceCommunicationBasics.Etx, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(command.Length, Is.EqualTo(5));
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