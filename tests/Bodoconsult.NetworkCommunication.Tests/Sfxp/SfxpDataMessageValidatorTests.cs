// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

namespace Bodoconsult.NetworkCommunication.Tests.Sfxp;

[TestFixture]
internal class SfxpDataMessageValidatorTests
{
    [Test]
    public void IsMessageValid_RawDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new SfxpDataMessageValidator();

        var msg = new RawInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_SdcpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new SfxpDataMessageValidator();

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
        var validator = new SfxpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_SfxpDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new SfxpDataMessageValidator();

        var msg = new SfxpInboundDataMessage
        {
            RawMessageData = new Memory<byte>([0x5])
        };

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_SfxpDataMessageEmpty_ReturnsFalse()
    {
        // Arrange 
        var validator = new SfxpDataMessageValidator();

        var msg = new SfxpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }
}