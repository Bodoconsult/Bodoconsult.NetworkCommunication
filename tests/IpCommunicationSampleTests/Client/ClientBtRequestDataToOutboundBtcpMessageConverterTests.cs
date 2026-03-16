// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Client.Bll.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSampleTests.Client;

[TestFixture]
internal class ClientBtRequestDataToOutboundBtcpMessageConverterTests
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
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        // Assert
        Assert.That(conv.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToBusinessTransactionRequestData_GetConfig_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.GetConfig;

        var request = new EmptyBusinessTransactionRequestData()
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpOutboundDataMessage)conv.MapToOutboundDataMessage(request);

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
    public void MapToBusinessTransactionRequestData_StartStreaming_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StartStreaming;

        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpOutboundDataMessage)conv.MapToOutboundDataMessage(request);

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
    public void MapToBusinessTransactionRequestData_StopStreaming_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StopStreaming;

        var request = new EmptyBusinessTransactionRequestData()
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpOutboundDataMessage)conv.MapToOutboundDataMessage(request);

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
    public void MapToBusinessTransactionRequestData_StartSnapshot_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StartSnapshot;

        var request = new EmptyBusinessTransactionRequestData()
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpOutboundDataMessage)conv.MapToOutboundDataMessage(request);

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
    public void MapToBusinessTransactionRequestData_StopSnapshot_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StopSnapshot;

        var request = new EmptyBusinessTransactionRequestData()
        {
            TransactionId = transactionId,
        };

        // Act  
        var result = (BtcpOutboundDataMessage)conv.MapToOutboundDataMessage(request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DataBlock, Is.Not.Null);
            Assert.That(result.DataBlock?.Data.Length, Is.Zero);
        }
    }
}