// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpReplyInboundDataMessageTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const int transactionId = 101;
        var transactionUid = Guid.NewGuid();

        // Act  
        var msg = new BtcpReplyInboundDataMessage(transactionId, transactionUid);

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

    [Test]
    public void CheckReceivedMessage_ValidSetup_ReturnsTrue()
    {
        // Arrange 
        const int transactionId = 101;
        var transactionUid = Guid.NewGuid();

        var msg = new BtcpReplyInboundDataMessage(transactionId, transactionUid);

        var sentMsg = new BtcpRequestOutboundDataMessage(transactionId, transactionUid);

        IList<string> errors = new List<string>();

        // Act  
        var result = msg.CheckReceivedMessage(sentMsg, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void CheckReceivedMessage_NotFittingToSentMessage_ReturnsFalse()
    {
        // Arrange 
        const int transactionId = 101;
        var transactionUid = Guid.NewGuid();

        var msg = new BtcpReplyInboundDataMessage(transactionId, transactionUid);

        var sentMsg = new BtcpRequestOutboundDataMessage(transactionId + 1, transactionUid);

        IList<string> errors = new List<string>();

        // Act  
        var result = msg.CheckReceivedMessage(sentMsg, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
        }
    }
}