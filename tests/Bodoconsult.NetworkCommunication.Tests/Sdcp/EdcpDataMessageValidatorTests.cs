// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class EdcpDataMessageValidatorTests
{

    [Test]
    public void IsMessageValid_RawDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new EdcpDataMessageValidator();

        var msg = new RawInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_ScdpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new EdcpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_EcdpDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new EdcpDataMessageValidator();

        var msg = new EdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }
}