// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Reflection.Metadata;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;
using NuGet.Frameworks;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class SdcpDataMessageValidatorTests
{

    [Test]
    public void IsMessageValid_RawDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new SdcpDataMessageValidator();

        var msg = new RawDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_ScdpDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new SdcpDataMessageValidator();

        var msg = new SdcpDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_EcdpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new SdcpDataMessageValidator();

        var msg = new EdcpDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }
}