// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpDataMessageValidatorTests
{
    [Test]
    public void IsMessageValid_RawDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new RawInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_SdcpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_TncpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_BtdpDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new BtcpInboundDataMessage(1, Guid.NewGuid());

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }
}