// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

namespace IpCommunicationSampleTests.Backend.Converters;

[TestFixture]
internal class ClientInboundBtcpMessageToBtReplyConverterTests
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
        var converter = new ClientInboundBtcpMessageToBtReplyConverter(_appLogger);

        // Assert
        Assert.That(converter.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToOutboundDataMessage_ValidReplyErrorCode0_PropsSetCorrectly()
    {
        // Arrange 
        var converter = new ClientInboundBtcpMessageToBtReplyConverter(_appLogger);

        const int transactionid = 1;
        var transactionuid = Guid.NewGuid();

        var msg = new BtcpReplyInboundDataMessage(transactionid, transactionuid)
        {
            ErrorCode = 99,
            InfoMessage = "Blubb",
            ErrorMessage = "Blabb"
        };

        // Act  
        var result = converter.MapToBusinessTransactionReply(msg);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result);
            Assert.That(result.RequestData, Is.Null);

            Assert.That(result.ErrorCode, Is.EqualTo(msg.ErrorCode));
            Assert.That(result.Message, Is.EqualTo(msg.InfoMessage));
            Assert.That(result.ExceptionMessage, Is.EqualTo(msg.ErrorMessage));
        }
    }
}