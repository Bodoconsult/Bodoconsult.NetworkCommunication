// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;
using IpCommunicationSample.Client.Bll.BusinessLogic.Converters;
using IpCommunicationSample.Common.BusinessTransactions;
using System.Text;

namespace IpCommunicationSampleTests.Client.Converters;

[TestFixture]
internal class BackendBtReplyToOutboundDataMessageConverterTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();

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
        var converter = new BackendBtReplyToOutboundDataMessageConverter(_appLogger);

        // Assert
        Assert.That(converter.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToOutboundDataMessage_ValidReplyErrorCode0_PropsSetCorrectly()
    {

        // Arrange 
        var converter = new BackendBtReplyToOutboundDataMessageConverter(_appLogger);

        var request = new EmptyBusinessTransactionRequestData()
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