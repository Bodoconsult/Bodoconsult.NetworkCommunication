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
        byte[] data = [0x5, 0x6, 0x7, 0x8, 0x9, 0x1, 0x2, 0x3];
        const byte channel = 1;
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new SfxpDataChunkInboundDataLogger(dataExportService)
        {
            Channel = channel
        };

        var dataBlock = new SfxpInboundDatablock
        {
            Data = new Memory<byte>(data),
            DataChunks =
            {
                new DataChunk { Channel = channel, Data = data },
                new DataChunk { Channel = channel, Data = data },
                new DataChunk { Channel = channel, Data = data }
            }
        };

        var msg = new SfxpInboundDataMessage
        {
            DataBlock = dataBlock
        };

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count, Is.EqualTo(1));

            var mem = result[0];

            Assert.That(mem.Length, Is.EqualTo(dataBlock.DataChunks.Count * data.Length));

            Assert.That(mem.Span[0], Is.EqualTo(0x5));
            Assert.That(mem.Span[1], Is.EqualTo(0x6));
            Assert.That(mem.Span[8], Is.EqualTo(0x5));
            Assert.That(mem.Span[9], Is.EqualTo(0x6));
            Assert.That(mem.Span[16], Is.EqualTo(0x5));
            Assert.That(mem.Span[17], Is.EqualTo(0x6));
        }
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
    public void LogTheMessage_ValidMessage1_DataLogged()
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dataExportService.WasLogged, Is.True);
            Assert.That(dataExportService.BytesLogged, Is.EqualTo(data.Length));
        }
    }

    [Test]
    public void LogTheMessage_ValidMessage2_DataLogged()
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dataExportService.WasLogged, Is.True);
            Assert.That(dataExportService.BytesLogged, Is.EqualTo(data.Length));
        }
    }
}