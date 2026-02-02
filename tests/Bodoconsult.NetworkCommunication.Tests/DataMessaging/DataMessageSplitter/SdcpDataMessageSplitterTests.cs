// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageSplitter;

[TestFixture]
internal class SdcpDataMessageSplitterTests
{
    private readonly SdcpDataMessageSplitter _splitter = new();

    [Test]
    public void TryReadCommand_NoValidMessage_NullReturned()
    {
        // Arrange 
        var data = new byte[] { 0x99,  0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryReadCommand_ValidDataMessage_CommandReturned()
    {
        // Arrange 
        var data = new byte[] { DeviceCommunicationBasics.Stx, 0x99, 0x99, DeviceCommunicationBasics.Etx, 0x99 };
        var ros = new ReadOnlySequence<byte>(data);

        // Act  
        var result = _splitter.TryReadCommand(ref ros, out var command);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.EqualTo(4));

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
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.EqualTo(1));
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
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.EqualTo(1));
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
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.EqualTo(1));
    }
}