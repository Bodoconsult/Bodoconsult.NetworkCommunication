// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Edcp;

[TestFixture]
internal class EdcpHandshakeDataMessageValidatorTests
{
    [Test]
    public void IsHandshakeForSentMessage_AckHandshake_MessageIsValid()
    {
        // Arrange
        var msg = new EdcpOutboundDataMessage
        {
            BlockCode = 0x3
        };

        var handshake = new EdcpInboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Ack,
            BlockCode = msg.BlockCode
        };

        var validator = new EdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_CanHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new EdcpOutboundDataMessage
        {
            BlockCode = 0x3
        };

        var handshake = new EdcpInboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Can,
            BlockCode = msg.BlockCode
        };

        var validator = new EdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_NackHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new EdcpOutboundDataMessage
        {
            BlockCode = 0x3
        };

        var handshake = new EdcpInboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack,
            BlockCode = msg.BlockCode
        };

        var validator = new EdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_WrongBlockCode_MessageIsNotValid()
    {
        // Arrange 
        var msg = new EdcpOutboundDataMessage
        {
            BlockCode = 0x3
        };

        var handshake = new EdcpInboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack,
            BlockCode = 0x2
        };

        var validator = new BtcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }
}