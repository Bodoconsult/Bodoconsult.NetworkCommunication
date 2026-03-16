// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Edcp;

[TestFixture]
internal class TncpDataMessageCodecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        // Act  
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Assert
        Assert.That(codec.DataBlockCodingProcessor, Is.Not.Null);
    }

    [Test]
    public void DecodeDataMessage_ValidInput_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31, 0x4, 0x6c, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Zero);

        var TncpMsg = (TncpInboundDataMessage)result.DataMessage;

        Assert.That(TncpMsg.BlockCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void DecodeDataMessage_InvalidInput_MessageNotDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

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
        byte blockId = 0x31;

        var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

        var dataBlock = new BasicOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var msg = new TncpOutboundDataMessage
        {
            BlockCode = blockId,
            DataBlock = dataBlock
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Zero);
        Assert.That(msg.RawMessageData.Length, Is.Not.EqualTo(0));

        Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
        Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x31));
        Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlock_MessageEncoded()
    {
        // Arrange 
        byte blockId = 0x31;

        var msg = new TncpOutboundDataMessage
        {
            BlockCode = blockId
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new  TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Not.EqualTo(0));
        Assert.That(msg.RawMessageData.Length, Is.Zero);
    }
}