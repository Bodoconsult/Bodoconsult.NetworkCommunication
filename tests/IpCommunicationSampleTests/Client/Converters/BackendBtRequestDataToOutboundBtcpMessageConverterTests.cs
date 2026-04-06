// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpClient.Bll.BusinessLogic.Converters;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSampleTests.Client.Converters;

[TestFixture]
internal class BackendBtRequestDataToOutboundBtcpMessageConverterTests
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
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        // Assert
        Assert.That(conv.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToBusinessTransactionRequestData_GetConfig_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.GetConfig;

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StartStreaming_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StartStreaming;
        var transactionUid = Guid.NewGuid();

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
            TransactionGuid = transactionUid
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StopStreaming_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StopStreaming;
        var transactionUid = Guid.NewGuid();

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
            TransactionGuid = transactionUid
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StartSnapshot_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StartSnapshot;
        var transactionUid = Guid.NewGuid();

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
            TransactionGuid = transactionUid
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StopSnapshot_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StopSnapshot;
        var transactionUid = Guid.NewGuid();

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
            TransactionGuid = transactionUid
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_NotificationFired_ReturnsRequestMessage()
    {
        // Arrange 
        var conv = new BackendBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ServerSideBusinessTransactionIds.NotificationFired;
        var transactionUid = Guid.NewGuid();

        var request = new StateChangedEventFiredBusinessTransactionRequestData
        {
            TransactionId = transactionId,
            TransactionGuid = transactionUid,
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Act  
        var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result.DataBlock);
            Assert.That(result.DataBlock.Data.Length, Is.Not.Zero);

            var s = Encoding.UTF8.GetString(result.DataBlock.Data.Span);
            Assert.That(s, Is.EqualTo($"s1\u0005Blubb\u00052\u0005Blabb\u00053\u0005Blobb"));
        }
    }
}