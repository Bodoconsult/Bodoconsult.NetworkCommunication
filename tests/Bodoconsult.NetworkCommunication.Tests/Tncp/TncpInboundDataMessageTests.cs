// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Tncp;

[TestFixture]
internal class TncpInboundDataMessageTests
{

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var msg = new TncpInboundDataMessage();

        // Assert
        Assert.That(msg.MessageId, Is.Not.EqualTo(0));
        Assert.That(string.IsNullOrEmpty(msg.ToInfoString()), Is.False);
        Assert.That(string.IsNullOrEmpty(msg.ToShortInfoString()), Is.False);
    }
}