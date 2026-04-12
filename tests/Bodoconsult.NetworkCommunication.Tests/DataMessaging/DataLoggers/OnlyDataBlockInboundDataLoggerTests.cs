// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataExportServices;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataLoggers;

[TestFixture]
internal class OnlyDataBlockInboundDataLoggerTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        // Act  
        var logger = new OnlyDataBlockInboundDataLogger(dataExportService);

        // Assert
        Assert.That(logger.DataExportService, Is.EqualTo(dataExportService));
    }

    [Test]
    public void CheckIfMessageIsToLog_ValidMessage_ReturnsTrue()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new OnlyDataBlockInboundDataLogger(dataExportService);

        var dataBlock = new BasicInboundDatablock
            { Data = new Memory<byte>([0x5, 0x6, 0x7]) };

        var msg = new SdcpSortableInboundDataMessage
        {
            DataBlock = dataBlock
        };

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckIfMessageIsToLog_MessageWithoutDatablock_ReturnFals()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new OnlyDataBlockInboundDataLogger(dataExportService);

        var msg = new SdcpSortableInboundDataMessage();

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void LogTheMessage_ValidMessage_DataLogged()
    {
        // Arrange 
        var dataExportService = new FakeDataExportService();

        var logger = new OnlyDataBlockInboundDataLogger(dataExportService);

        var dataBlock = new BasicInboundDatablock
            { Data = new Memory<byte>([0x5, 0x6, 0x7]) };

        var msg = new SdcpSortableInboundDataMessage
        {
            DataBlock = dataBlock
        };

        var result = logger.CheckIfMessageIsToLog(msg);

        Assert.That(result, Is.True);

        // Act  
        logger.LogTheMessage(msg);

        // Assert
        Wait.Until(() => dataExportService.WasLogged);
        Assert.That(dataExportService.WasLogged, Is.True);
    }
}