// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpInboundDataMessageTests
{

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const int transactionId = 101;
        var transactionUid = Guid.NewGuid();

        // Act  
        var msg = new BtcpInboundDataMessage(transactionId, transactionUid);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg.MessageId, Is.Not.Zero);
            Assert.That(string.IsNullOrEmpty(msg.ToInfoString()), Is.False);
            Assert.That(string.IsNullOrEmpty(msg.ToShortInfoString()), Is.False);

            Assert.That(msg.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(msg.BusinessTransactionUid, Is.EqualTo(transactionUid));
        }
    }
}