// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessagingProcessor;

[TestFixture]
internal class DefaultDataMessageCodingProcessorTests
{
    [Test]
    public void DecodeDataMessage_BtcpHandshake_HandshakeMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new BtcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new BtcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x6 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(InboundHandshakeMessage)));
        }
    }

    [Test]
    public void DecodeDataMessage_BtcpDataMessage_DataMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new BtcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new BtcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x2, 0x1, 0x31, 0x4, 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(BtcpInboundDataMessage)));
        }
    }

    [Test]
    public void EncodeDataMessag_BtcpDataMessage_RawBytesCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new BtcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new BtcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62 };

        var datablock = new BasicOutboundDatablock
        {
            Data = msg
        };

        var message = new BtcpOutboundDataMessage(99)
        {
            DataBlock = datablock
        };

        // Act  
        var result = processor.EncodeDataMessage(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(message.RawMessageData.Length, Is.Not.EqualTo(0));
        }
    }

    [Test]
    public void DecodeDataMessage_SdcpHandshake_HandshakeMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new SdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new SdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x6 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(InboundHandshakeMessage)));
        }
    }

    [Test]
    public void DecodeDataMessage_SdcpDataMessage_DataMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new SdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new SdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x2, 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(SdcpInboundDataMessage)));
        }
    }

    [Test]
    public void EncodeDataMessag_SdcpDataMessage_RawBytesCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new SdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new SdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62 };

        var datablock = new BasicOutboundDatablock
        {
            Data = msg
        };

        var message = new SdcpOutboundDataMessage
        {
            DataBlock = datablock
        };

        // Act  
        var result = processor.EncodeDataMessage(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(message.RawMessageData.Length, Is.Not.EqualTo(0));
        }
    }

    [Test]
    public void DecodeDataMessage_EdcpHandshake_HandshakeMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new EdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new EdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x6, 0x5 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(EdcpInboundHandshakeMessage)));
        }
    }

    [Test]
    public void DecodeDataMessage_EdcpDataMessage_DataMessageCreated()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new EdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new EdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x2, 0x5, 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(EdcpInboundDataMessage)));
        }
    }

    [Test]
    public void EncodeDataMessag_EdcpDataMessage_RawBytesCreated()
    {
        // Arrange 
        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var processor = new DefaultDataMessageCodingProcessor();
        processor.MessageCodecs.Add(new EdcpHandshakeMessageCodec());
        processor.MessageCodecs.Add(new EdcpDataMessageCodec(dataBlockCodingProcessor));

        var msg = new byte[] { 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62 };

        var datablock = new BasicOutboundDatablock
        {
            Data = msg
        };

        var message = new EdcpOutboundDataMessage
        {
            DataBlock = datablock,
            BlockCode = 55
        };

        // Act  
        var result = processor.EncodeDataMessage(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(message.RawMessageData.Length, Is.Not.EqualTo(0));
        }
    }
}