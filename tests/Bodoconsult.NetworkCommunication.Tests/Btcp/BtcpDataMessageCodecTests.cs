// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpDataMessageCodecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());

        // Act  
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Assert
        Assert.That(codec.DataBlockCodingProcessor, Is.Not.Null);
    }

    [Test]
    public void DecodeDataMessage_ValidInputWithDataBlock_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31, 0x4, 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.EqualTo(0));

        var btcpMsg = (BtcpInboundDataMessage)result.DataMessage;

        Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
        Assert.That(btcpMsg.DataBlock, Is.Not.Null);
        Assert.That(btcpMsg.DataBlock.Data.Length, Is.EqualTo(11));
    }

    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockWithEot_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31, 0x4, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.EqualTo(0));

        var btcpMsg = (BtcpInboundDataMessage)result.DataMessage;

        Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
        Assert.That(btcpMsg.DataBlock, Is.Null);
    }

    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockNoEot_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31, 0x4, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.EqualTo(0));

        var btcpMsg = (BtcpInboundDataMessage)result.DataMessage;

        Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
        Assert.That(btcpMsg.DataBlock, Is.Null);
    }

    [Test]
    public void DecodeDataMessage_InvalidInput_MessageNotDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Not.EqualTo(0));
        Assert.That(result.DataMessage, Is.Null);
    }

    [Test]
    public void EncodeDataMessage_ValidInput_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;

        var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

        var dataBlock = new DummyOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var msg = new BtcpOutboundDataMessage(transactionId)
        {
            DataBlock = dataBlock
        };

        Assert.That(msg.RawMessageData.Length, Is.EqualTo(0));

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.EqualTo(0));
        Assert.That(msg.RawMessageData.Length, Is.Not.EqualTo(0));

        Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
        Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x31));
        Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(DeviceCommunicationBasics.Eot));

        Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlock_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;

        var msg = new BtcpOutboundDataMessage(transactionId);

        Assert.That(msg.RawMessageData.Length, Is.EqualTo(0));

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.EqualTo(0));
        Assert.That(msg.RawMessageData.Length, Is.Not.EqualTo(0));

        Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
        Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x31));
        Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(DeviceCommunicationBasics.Etx));
    }
}