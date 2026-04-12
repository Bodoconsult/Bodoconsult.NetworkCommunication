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
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new SfxpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);

            Assert.That(result.DataMessage, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result.DataMessage);
            Assert.That(result.DataMessage.RawMessageData.Length, Is.Not.Zero);

            var msg2 = (SfxpInboundDataMessage)result.DataMessage;

            Assert.That(msg2, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(msg2.DataBlock);

            if (number == "0")
            {
                Assert.That(msg2.DataBlock.Data.Span[0], Is.EqualTo(DeviceCommunicationBasics.Null));
            }

            Assert.That(msg2.DataBlock.Data.Length, Is.EqualTo(msg.Length-8));
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

    [TestCase("0", TestName = "EncodeDataMessage_Sfx0_MessageEncoded")]
    [TestCase("1", TestName = "EncodeDataMessage_Sfx1_MessageEncoded")]
    [TestCase("2", TestName = "EncodeDataMessage_Sfx2_MessageEncoded")]
    [TestCase("3", TestName = "EncodeDataMessage_Sfx3_MessageEncoded")]
    [TestCase("4", TestName = "EncodeDataMessage_Sfx4_MessageEncoded")]
    [TestCase("5", TestName = "EncodeDataMessage_Sfx5_MessageEncoded")]
    [TestCase("6", TestName = "EncodeDataMessage_Sfx6_MessageEncoded")]
    [TestCase("7", TestName = "EncodeDataMessage_Sfx7_MessageEncoded")]
    [TestCase("8", TestName = "EncodeDataMessage_Sfx8_MessageEncoded")]
    [TestCase("9", TestName = "EncodeDataMessage_Sfx9_MessageEncoded")]
    public void EncodeDataMessage_Sfx_MessageEncoded(string number)
    {
        // Arrange 
        var data = ResourceHelper.GetByteResource($"Bodoconsult.NetworkCommunication.Tests.Resources.sfx{number}.bin");

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
            Assert.That(msg.RawMessageData.Length, Is.EqualTo(data.Length));
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