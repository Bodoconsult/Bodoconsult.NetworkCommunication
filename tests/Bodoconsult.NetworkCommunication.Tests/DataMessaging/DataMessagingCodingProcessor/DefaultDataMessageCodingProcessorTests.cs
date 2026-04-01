// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessagingCodingProcessor;

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
            ArgumentNullException.ThrowIfNull(result.DataMessage);
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

        var msg = new byte[] { 0x2, 0x0, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64,
            0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35,
            0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4,
            // Identifier byte
            0x78,
            // Default reply
            0x32, 0x7c, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x7c, 0x42, 0x6c, 0x61, 0x62, 0x62, 0x7c,
            // Payload
            0x42, 0x6c, 0x69, 0x70, 0x70,
            0x3 };

        // Act  
        var result = processor.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DataMessage, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result.DataMessage);
            Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(BtcpReplyInboundDataMessage)));
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

        var transactionUid = Guid.NewGuid();

        var message = new BtcpRequestOutboundDataMessage(99, transactionUid)
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
            Assert.That(message.RawMessageData.Length, Is.Not.Zero);
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
            ArgumentNullException.ThrowIfNull(result.DataMessage);
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
            ArgumentNullException.ThrowIfNull(result.DataMessage);
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
            Assert.That(message.RawMessageData.Length, Is.Not.Zero);
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
            ArgumentNullException.ThrowIfNull(result.DataMessage);
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
            ArgumentNullException.ThrowIfNull(result.DataMessage);
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
            Assert.That(message.RawMessageData.Length, Is.Not.Zero);
        }
    }
}