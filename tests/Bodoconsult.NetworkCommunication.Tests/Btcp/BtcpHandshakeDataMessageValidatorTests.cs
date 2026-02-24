// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpHandshakeDataMessageValidatorTests
{
    [Test]
    public void IsHandshakeForSentMessage_AckHandshake_MessageIsValid()
    {
        // Arrange 
        const int transactionId = 101;
        
        var msg = new BtcpOutboundDataMessage(transactionId);

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Ack
        };

        var validator = new BtcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_CanHandshake_MessageIsValid()
    {
        // Arrange 
        const int transactionId = 101;

        var msg = new BtcpOutboundDataMessage(transactionId);

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Can
        };

        var validator = new BtcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsHandshakeForSentMessage_NackHandshake_MessageIsValid()
    {
        // Arrange 
        const int transactionId = 101;

        var msg = new BtcpOutboundDataMessage(transactionId);

        var handshake = new InboundHandshakeMessage
        {
            HandshakeMessageType = DeviceCommunicationBasics.Nack
        };

        var validator = new BtcpHandshakeDataMessageValidator();

        // Act  
        var result = validator.IsHandshakeForSentMessage(msg, handshake);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }


}