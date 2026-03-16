// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Client.Bll.BusinessTransactions;
using IpCommunicationSample.Common;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using System.Text;

namespace IpCommunicationSampleTests.Client;

[TestFixture]
internal class ClientInboundBtcpMessageToBtRequestDataConverterTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}

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
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        // Assert
        Assert.That(conv.AppLogger, Is.EqualTo(_appLogger));
    }

    [Test]
    public void MapToBusinessTransactionRequestData_ValidMessage_ReturnsRequestData()
    {
        // Arrange 
        var conv = new ClientInboundBtcpMessageToBtRequestDataConverter(_appLogger);

        const int transactionId = 1;

        const int deviceStateId = 2;
        const int businessStateId = 3;
        const int businessSubstateId = 4;
        const string deviceStateName = "Blubb";
        const string businessStateName = "Blubb";
        const string businessSubstateName = "Blebb";

        var datablock = new BasicInboundDatablock
        {
            DataBlockType = DataBlockTypes.StateChangedEventFiredBusiness,
            Data = Encoding.UTF8.GetBytes(
                $"{deviceStateId}\u0005{deviceStateName}\u0005{businessStateId}\u0005{businessStateName}\u0005{businessSubstateId}\u0005{businessSubstateName}")
        };

        var message = new BtcpInboundDataMessage(transactionId)
        {
            BusinessTransactionId = transactionId,
            DataBlock = datablock,
            IsRequest = true
        };

        // Act  
        var result = (StateChangedEventFiredBusinessTransactionRequestData?)conv.MapToBusinessTransactionRequestData(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(result);

            Assert.That(result.TransactionId, Is.EqualTo(transactionId));
            Assert.That(result.DeviceStateId, Is.EqualTo(deviceStateId));
            Assert.That(result.DeviceStateName, Is.EqualTo(deviceStateName));

            Assert.That(result.BusinessStateId, Is.EqualTo(businessStateId));
            Assert.That(result.BusinessStateName, Is.EqualTo(businessStateName));

            Assert.That(result.BusinessSubstateId, Is.EqualTo(businessSubstateId));
            Assert.That(result.BusinessSubstateName, Is.EqualTo(businessSubstateName));
        }
    }
}