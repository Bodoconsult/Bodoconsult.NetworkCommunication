// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataExportServices;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataLoggers;

[TestFixture]
internal class SfxpDataChunkInboundDataLoggerTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        // Act  
        var logger = new SfxpDataChunkInboundDataLogger(dataExportService);

        // Assert
        Assert.That(logger.DataExportService, Is.EqualTo(dataExportService));
    }

    [Test]
    public void CheckIfMessageIsToLog_ValidMessage_ReturnsTrue()
    {
        // Arrange 
        byte[] data = [0x5, 0x6, 0x7];
        const byte channel = 1;
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new SfxpDataChunkInboundDataLogger(dataExportService)
        {
            Channel = channel
        };

        var dataBlock = new SfxpInboundDatablock
        {
            Data = new Memory<byte>(data),
            DataChunks = { new DataChunk { Channel = channel, Data = data } }
        };

        var msg = new SfxpInboundDataMessage
        {
            DataBlock = dataBlock
        };

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public void CheckIfMessageIsToLog_MessageWithoutDatablock_ReturnFalse()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();
        const byte channel = 1;

        var logger = new SfxpDataChunkInboundDataLogger(dataExportService)
        {
            Channel = channel
        };

        var msg = new SfxpInboundDataMessage();

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result.Count, Is.Zero);
    }

    [Test]
    public void CheckIfMessageIsToLog_MessageNotFittingChunks_ReturnFalse()
    {
        // Arrange 
        byte[] data = [0x5, 0x6, 0x7];
        const byte channel = 1;
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new SfxpDataChunkInboundDataLogger(dataExportService)
        {
            Channel = channel
        };

        var dataBlock = new SfxpInboundDatablock
        {
            Data = new Memory<byte>(data),
            DataChunks = { new DataChunk { Channel = 0xff, Data = data } }
        };

        var msg = new SfxpInboundDataMessage
        {
            DataBlock = dataBlock
        };

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result.Count, Is.Zero);
    }
    [Test]
    public void LogTheMessage_ValidMessage_DataLogged()
    {
        // Arrange 
        byte[] data = [0x5, 0x6, 0x7];
        const byte channel = 1;
        var dataExportService = new FakeDataExportService();

        var logger = new SfxpDataChunkInboundDataLogger(dataExportService)
        {
            Channel = channel
        };

        var dataBlock = new SfxpInboundDatablock
        {
            Data = new Memory<byte>(data),
            DataChunks = { new DataChunk { Channel = channel, Data = data } }
        };

        var msg = new SfxpInboundDataMessage
        {
            DataBlock = dataBlock
        };

        var result = logger.CheckIfMessageIsToLog(msg);

        // Act  
        logger.LogTheMessages(result);

        // Assert
        Wait.Until(() => dataExportService.WasLogged);
        Assert.That(dataExportService.WasLogged, Is.True);
    }
}