// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpOutboundDataMessageTests
{

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const int transactionId = 101;

        // Act  
        var msg = new BtcpOutboundDataMessage(transactionId);

        // Assert
        Assert.That(msg.MessageId, Is.Not.EqualTo(0));
        Assert.That(string.IsNullOrEmpty(msg.ToInfoString()), Is.False);
        Assert.That(string.IsNullOrEmpty(msg.ToShortInfoString()), Is.False);

        Assert.That(msg.BusinessTransactionId, Is.EqualTo(transactionId));
    }
}