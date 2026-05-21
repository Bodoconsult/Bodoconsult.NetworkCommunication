// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.Converters;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSampleTests.Backend.Converters;

[TestFixture]
internal class ClientInboundBtcpMessageToBtRequestDataConverterTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}

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
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        // Assert
        Assert.That(conv.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToBusinessTransactionRequestData_GetConfig_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.GetConfig;
        var transactionUid = Guid.NewGuid();

        var db = new BasicInboundDatablock
        {
            Data = new Memory<byte>([0x0, 0x1, 0x0, 0x0, 0x0])
        };

        var message = new BtcpRequestInboundDataMessage(transactionId, transactionUid)
        {
            DataBlock = db
        };

        // Act  
        var result = (StartMessagingBusinessTransactionRequestData?)conv.MapToBusinessTransactionRequestData(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionId, Is.EqualTo(transactionId));
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StartStreaming_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StartMessaging;
        var transactionUid = Guid.NewGuid();

        var db = new BasicInboundDatablock
        {
            Data = new Memory<byte>([0x0, 0x1, 0x0, 0x0, 0x0])
        };

        var message = new BtcpRequestInboundDataMessage(transactionId, transactionUid)
        {
            DataBlock = db
        };


        // Act  
        var result = (StartMessagingBusinessTransactionRequestData?)conv.MapToBusinessTransactionRequestData(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionId, Is.EqualTo(transactionId));
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_StopStreaming_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.StopMessaging;
        var transactionUid = Guid.NewGuid();

        var db = new BasicInboundDatablock
        {
            Data = new byte[]{ 0x0}
        };

        var message = new BtcpRequestInboundDataMessage(transactionId, transactionUid)
        {
            DataBlock = db
        };

        // Act  
        var result = (EmptyBusinessTransactionRequestData?)conv.MapToBusinessTransactionRequestData(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionId, Is.EqualTo(transactionId));
        }
    }

    [Test]
    public void MapToBusinessTransactionRequestData_CreateFftAnalysisReport_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        var transactionId = ClientSideBusinessTransactionIds.CreateFftAnalysisReport;
        var transactionUid = Guid.NewGuid();

        var message = new BtcpRequestInboundDataMessage(transactionId, transactionUid);

        // Act  
        var result = (FftReportBusinessTransactionRequestData ?)conv.MapToBusinessTransactionRequestData(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.TransactionId, Is.EqualTo(transactionId));
        }
    }
}