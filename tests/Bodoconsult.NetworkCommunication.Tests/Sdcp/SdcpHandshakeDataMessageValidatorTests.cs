// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class SdcpHandshakeDataMessageValidatorTests
{
    [Test]
    public void IsHandshakeForSentMessage_AckHandshake_MessageIsValid()
    {
        // Arrange
        var msg = new SdcpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Ack
        };

        var validator = new SdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_CanHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new SdcpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Can
        };

        var validator = new SdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_NackHandshake_MessageIsValid()
    {
        // Arrange 
        var msg = new SdcpOutboundDataMessage();

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack
        };

        var validator = new SdcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }
}