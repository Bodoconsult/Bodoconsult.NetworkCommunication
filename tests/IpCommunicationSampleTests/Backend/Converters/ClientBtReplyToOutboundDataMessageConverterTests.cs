// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.Converters;
using IpCommunicationSample.Common.BusinessTransactions.Replies;

namespace IpCommunicationSampleTests.Backend.Converters;

[TestFixture]
internal class ClientBtReplyToOutboundDataMessageConverterTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.Logger;

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var converter = new ClientBtReplyToOutboundDataMessageConverter(_appLogger);

        // Assert
        Assert.That(converter.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToOutboundDataMessage_ValidDefaultBusinessTransactionReplyErrorCode0_PropsSetCorrectly()
    {
        // Arrange 
        var converter = new ClientBtReplyToOutboundDataMessageConverter(_appLogger);

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = 100,
            TransactionGuid = Guid.NewGuid()
        };

        var reply = new DefaultBusinessTransactionReply
        {
            RequestData = request
        };

        // Act  
        var msg = converter.MapToOutboundDataMessage(reply);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg);

            Assert.That(msg.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg.DataBlock);

            var payload = msg.DataBlock.Data;

            Assert.That(payload.Length, Is.Not.Zero);

            var dataString = Encoding.UTF8.GetString(payload.Span);

            Assert.That(dataString, Is.EqualTo("0||"));

            var replyMsg = (BtcpReplyOutboundDataMessage)msg;
            Assert.That(replyMsg.BusinessTransactionId, Is.EqualTo(request.TransactionId));
            Assert.That(replyMsg.BusinessTransactionUid, Is.EqualTo(request.TransactionGuid));
        }
    }

    [Test]
    public void MapToOutboundDataMessage_ValidFftReportBusinessTransactionReplyErrorCode0_PropsSetCorrectly()
    {
        // Arrange 
        const int errorCode = 0;
        var converter = new ClientBtReplyToOutboundDataMessageConverter(_appLogger);

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = 100,
            TransactionGuid = Guid.NewGuid()
        };

        var reply = new FftReportBusinessTransactionReply
        {
            RequestData = request,
            ErrorCode = errorCode
        };

        // Act  
        var msg = converter.MapToOutboundDataMessage(reply);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(msg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg);

            Assert.That(msg.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg.DataBlock);

            var payload = msg.DataBlock.Data;

            Assert.That(payload.Length, Is.Not.Zero);

            var dataString = Encoding.UTF8.GetString(payload.Span);

            Assert.That(dataString, Is.EqualTo("0||"));

            var replyMsg = (BtcpReplyOutboundDataMessage)msg;
            Assert.That(replyMsg.BusinessTransactionId, Is.EqualTo(request.TransactionId));
            Assert.That(replyMsg.BusinessTransactionUid, Is.EqualTo(request.TransactionGuid));
        }
    }
}