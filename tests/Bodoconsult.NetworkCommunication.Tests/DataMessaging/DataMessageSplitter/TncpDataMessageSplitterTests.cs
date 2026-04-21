// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
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
        var data = new byte[] { 0x1, 0x99, 0x99, DeviceCommunicationBasics.Cr, 0x99 };
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
            0x31, 0xd, 0x99
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