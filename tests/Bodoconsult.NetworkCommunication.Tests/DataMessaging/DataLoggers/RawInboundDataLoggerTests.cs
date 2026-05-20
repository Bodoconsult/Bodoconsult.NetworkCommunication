// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataExportServices;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataLoggers;

[TestFixture]
internal class RawInboundDataLoggerTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        // Act  
        var logger = new RawInboundDataLogger(dataExportService);

        // Assert
        Assert.That(logger.DataExportService, Is.EqualTo(dataExportService));
    }

    [Test]
    public void CheckIfMessageIsToLog_ValidMessage_ReturnsTrue()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new RawInboundDataLogger(dataExportService);

        var dataBlock = new BasicInboundDatablock
        { Data = new Memory<byte>([0x5, 0x6, 0x7]) };

        var msg = new SdcpSortableInboundDataMessage
        {
            DataBlock = dataBlock,
            RawMessageData = dataBlock.Data
        };

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result.Count, Is.Not.Zero);
    }

    [Test]
    public void CheckIfMessageIsToLog_ValidMessage_ReturnsFalse()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new RawInboundDataLogger(dataExportService);

        var msg = new SdcpSortableInboundDataMessage();

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result.Count, Is.Zero);
    }

    [Test]
    public void LogTheMessage_ValidMessage_DataLogged()
    {
        // Arrange 
        var dataExportService = new FakeDataExportService();

        var logger = new RawInboundDataLogger(dataExportService);

        var msg = new SdcpSortableInboundDataMessage();

        var result = logger.CheckIfMessageIsToLog(msg);

        // Act  
        logger.LogTheMessages(result);

        // Assert
        Wait.Until(() => dataExportService.WasLogged);
        Assert.That(dataExportService.WasLogged, Is.True);
    }
}