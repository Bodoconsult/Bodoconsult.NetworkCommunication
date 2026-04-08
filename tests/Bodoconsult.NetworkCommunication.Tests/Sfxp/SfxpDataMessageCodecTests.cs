// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Sfxp;

[TestFixture]
internal class SfxpDataMessageCodecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();

        // Act  
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Assert
        Assert.That(codec.DataBlockCodingProcessor, Is.Not.Null);
    }

    //[Test]
    //public void DecodeDataMessage_Sfx0_MessageDecoded()
    //{
    //    // Arrange 
    //    var msg = ResourceHelper.GetByteResource("Bodoconsult.NetworkCommunication.Tests.Resources.sfx0.bin");

    //    IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
    //    var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

    //    // Act  
    //    var result = codec.DecodeDataMessage(msg);

    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(result.ErrorCode, Is.Zero);
    //    }
    //}

    [TestCase("0", TestName = "DecodeDataMessage_Sfx0_MessageDecoded")]
    [TestCase("1", TestName = "DecodeDataMessage_Sfx1_MessageDecoded")]
    [TestCase("2", TestName = "DecodeDataMessage_Sfx2_MessageDecoded")]
    [TestCase("3", TestName = "DecodeDataMessage_Sfx3_MessageDecoded")]
    [TestCase("4", TestName = "DecodeDataMessage_Sfx4_MessageDecoded")]
    [TestCase("5", TestName = "DecodeDataMessage_Sfx5_MessageDecoded")]
    [TestCase("6", TestName = "DecodeDataMessage_Sfx6_MessageDecoded")]
    [TestCase("7", TestName = "DecodeDataMessage_Sfx7_MessageDecoded")]
    [TestCase("8", TestName = "DecodeDataMessage_Sfx8_MessageDecoded")]
    [TestCase("9", TestName = "DecodeDataMessage_Sfx9_MessageDecoded")]
    public void DecodeDataMessage_Sfx_MessageDecoded(string number)
    {
        // Arrange 
        var msg = ResourceHelper.GetByteResource($"Bodoconsult.NetworkCommunication.Tests.Resources.sfx{number}.bin");

        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
        }
    }

    [Test]
    public void DecodeDataMessage_InvalidInput_MessageNotDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31 };

        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Not.Zero);
            Assert.That(result.DataMessage, Is.Null);
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInput_MessageEncoded()
    {
        // Arrange 
        var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

        var dataBlock = new BasicOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var msg = new SfxpOutboundDataMessage
        {
            DataBlock = dataBlock
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(msg.RawMessageData.Length, Is.EqualTo(data.Length + 3));

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlock_MessageEncoded()
    {
        // Arrange 
        var msg = new SfxpOutboundDataMessage();

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Not.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Zero);
        }
    }
}