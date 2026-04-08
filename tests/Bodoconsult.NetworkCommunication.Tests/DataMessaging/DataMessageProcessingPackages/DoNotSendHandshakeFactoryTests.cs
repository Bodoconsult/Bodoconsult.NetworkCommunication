// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageProcessingPackages;

[TestFixture]
internal class DoNotSendHandshakeFactoryTests
{
    [Test]
    public void GetAckResponse_ValidMessage_ReturnsAckHandshake()
    {
        // Arrange 
        var factory = new DoNotSendHandshakeFactory();

        var message = new SfxpInboundDataMessage();

        // Act  
        var result = factory.GetAckResponse(message);

        // Assert
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(DoNotSendOutboundHandshakeMessage)));
    }

    [Test]
    public void GetNackResponse_ValidMessage_ReturnsNackHandshake()
    {
        // Arrange 
        var factory = new DoNotSendHandshakeFactory();

        var message = new SfxpInboundDataMessage();

        // Act  
        var result = factory.GetNackResponse(message);

        // Assert
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(DoNotSendOutboundHandshakeMessage)));
    }

    [Test]
    public void GetCanResponse_ValidMessage_ReturnsCanHandshake()
    {
        // Arrange 
        var factory = new DoNotSendHandshakeFactory();

        var message = new SfxpInboundDataMessage();

        // Act  
        var result = factory.GetCanResponse(message);

        // Assert
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(DoNotSendOutboundHandshakeMessage)));
    }
}