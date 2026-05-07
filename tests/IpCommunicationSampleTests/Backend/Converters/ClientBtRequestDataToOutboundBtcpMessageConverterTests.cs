// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.Converters;

namespace IpCommunicationSampleTests.Backend.Converters;

[TestFixture]
internal class ClientBtRequestDataToOutboundBtcpMessageConverterTests
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
        var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

        // Assert
        Assert.That(conv.AppLogger, Is.EqualTo(_appLogger));
    }

    //[Test]
    //public void MapToBusinessTransactionRequestData_StateChangedEventFired_ReturnsRequestData()
    //{
    //    // Arrange 
    //    var conv = new ClientBtRequestDataToOutboundBtcpMessageConverter(_appLogger);

    //    var transactionId = ServerSideBusinessTransactionIds.StateChangedEventFired;

    //    const int deviceStateId = 2;
    //    const int businessStateId = 3;
    //    const int businessSubstateId = 4;
    //    const string deviceStateName = "Blubb";
    //    const string businessStateName = "Blubb";
    //    const string businessSubstateName = "Blebb";


    //    var request = new StateMachineStateNotification
    //    {
    //        TransactionId = transactionId,
    //        DeviceStateId = deviceStateId,
    //        DeviceStateName = deviceStateName,
    //        BusinessStateId = businessStateId,
    //        BusinessStateName = businessStateName,
    //        BusinessSubstateId = businessSubstateId,
    //        BusinessSubstateName = businessSubstateName
    //    };

    //    var expectedPayload = Encoding.UTF8.GetBytes($"{deviceStateId}\u0005{deviceStateName}\u0005{businessStateId}\u0005{businessStateName}\u0005{businessSubstateId}\u0005{businessSubstateName}");

    //    // Act  
    //    var result = (BtcpRequestOutboundDataMessage)conv.MapToOutboundDataMessage(request);

    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(result.BusinessTransactionId, Is.EqualTo(transactionId));
    //        Assert.That(result.DataBlock, Is.Not.Null);
    //        Assert.That(result.DataBlock?.DataBlockType, Is.EqualTo(DataBlockTypes.StateChangedEventFiredBusiness));
    //        Assert.That(result.DataBlock?.Data.IsEqualTo(expectedPayload), Is.True);
    //    }
    //}
}