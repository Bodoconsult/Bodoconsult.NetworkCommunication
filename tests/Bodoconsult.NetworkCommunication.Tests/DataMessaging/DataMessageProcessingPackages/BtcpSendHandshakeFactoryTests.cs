// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageProcessingPackages;

[TestFixture]
internal class BtcpSendHandshakeFactoryTests
{
    [Test]
    public void GetAckResponse_ValidMessage_ReturnsAckHandshake()
    {
        // Arrange 
        var factory = new BtcpHandshakeFactory();

        var message = new BtcpRequestInboundDataMessage(1, Guid.NewGuid());

        // Act  
        var result = factory.GetAckResponse(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GetType().Name, Is.EqualTo(nameof(OutboundHandshakeMessage)));
            Assert.That(result.HandshakeMessageType, Is.EqualTo(DeviceCommunicationBasics.Ack));
        }
    }

    [Test]
    public void GetNackResponse_ValidMessage_ReturnsNackHandshake()
    {
        // Arrange 
        var factory = new BtcpHandshakeFactory();

        var message = new BtcpRequestInboundDataMessage(1, Guid.NewGuid());

        // Act  
        var result = factory.GetNackResponse(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GetType().Name, Is.EqualTo(nameof(OutboundHandshakeMessage)));
            Assert.That(result.HandshakeMessageType, Is.EqualTo(DeviceCommunicationBasics.Nack));
        }
    }

    [Test]
    public void GetCanResponse_ValidMessage_ReturnsCanHandshake()
    {
        // Arrange 
        var factory = new BtcpHandshakeFactory();

        var message = new BtcpRequestInboundDataMessage(1, Guid.NewGuid());

        // Act  
        var result = factory.GetCanResponse(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.GetType().Name, Is.EqualTo(nameof(OutboundHandshakeMessage)));
            Assert.That(result.HandshakeMessageType, Is.EqualTo(DeviceCommunicationBasics.Can));
        }
    }
}