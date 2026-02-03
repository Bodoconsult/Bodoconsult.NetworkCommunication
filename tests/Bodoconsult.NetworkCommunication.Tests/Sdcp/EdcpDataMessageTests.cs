// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class EdcpDataMessageTests
{

    [Test]
    public void Ctor_ValidSetuo_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var msg = new EdcpDataMessage();

        // Assert
        Assert.That(msg.MessageId, Is.Not.EqualTo(0));
        Assert.That(string.IsNullOrEmpty(msg.ToInfoString()), Is.False);
        Assert.That(string.IsNullOrEmpty(msg.ToShortInfoString()), Is.False);
    }
}