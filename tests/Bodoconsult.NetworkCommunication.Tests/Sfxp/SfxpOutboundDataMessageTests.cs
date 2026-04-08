// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Sfxp;

[TestFixture]
internal class SfxpOutboundDataMessageTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var msg = new SfxpOutboundDataMessage();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg.MessageId, Is.Not.Zero);
            Assert.That(string.IsNullOrEmpty(msg.ToInfoString()), Is.False);
            Assert.That(string.IsNullOrEmpty(msg.ToShortInfoString()), Is.False);
        }
    }
}