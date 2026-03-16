// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Edcp;

[TestFixture]
internal class TncpHandshakeDataMessageValidatorTests
{
    [Test]
    public void IsHandshakeForSentMessage_AckHandshake_MessageIsValid()
    {
        // Arrange
        var msg = new TncpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Ack,
        };

        var validator = new TncpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_CanHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new TncpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Can
        };

        var validator = new TncpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_NackHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new TncpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack
        };

        var validator = new TncpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_WrongBlockCode_MessageIsNotValid()
    {
        // Arrange 
        var msg = new TncpOutboundDataMessage();

        var handshake = new EdcpInboundHandshakeMessage()
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack
        };

        var validator = new TncpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }
}