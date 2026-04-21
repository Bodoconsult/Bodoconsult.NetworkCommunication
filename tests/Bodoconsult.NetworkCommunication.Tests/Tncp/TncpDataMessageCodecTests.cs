// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Tests.Tncp;

[TestFixture]
internal class TncpDataMessageCodecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
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
        var cmd = "log,charstat";

        var msg = Encoding.UTF8.GetBytes($"{cmd}\u0013");

        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);

            ArgumentNullException.ThrowIfNull(result.DataMessage);

            Assert.That(result.DataMessage, Is.Not.Null);

            var tncpMsg = (TncpInboundDataMessage)result.DataMessage;

            Assert.That(tncpMsg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo(cmd));
        }
    }

    [Test]
    public void DecodeDataMessage_InvalidInput_MessageNotDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x13 };

        IDataBlockCodingProcessor dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

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
    public void EncodeDataMessage_ValidDataBlockInput_MessageEncoded()
    {
        // Arrange 
        const string cmd = "log,charstat";

        var dataBlock = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty,
            DataBlockType = 'x'
        };

        var msg = new TncpOutboundDataMessage
        {
            DataBlock = dataBlock,
            TelnetCommand = cmd
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(msg.RawMessageData.Length, Is.EqualTo(cmd.Length + 1));

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(cmd[0]));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(cmd[1]));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1],
                Is.EqualTo(DeviceCommunicationBasics.Cr));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidParameterSetInput_MessageEncoded()
    {
        // Arrange 
        const string cmd = "log,charstat";

        var ps = new TncpParameterSet();
        ps.TelnetCommand = cmd;

        var msg = new TncpOutboundDataMessage
        {
            DataBlock = ps
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(msg.RawMessageData.Length, Is.EqualTo(cmd.Length + 1));

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(cmd[0]));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(cmd[1]));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1],
                Is.EqualTo(DeviceCommunicationBasics.Cr));
        }
    }

    [Test]
    public void EncodeDataMessage_InalidParameterSetInput_MessageNotEncoded()
    {
        // Arrange 
        const string cmd = "log,charstat";

        var ps = new TncpParameterSet();
        ps.TelnetCommand = null;

        var msg = new TncpOutboundDataMessage
        {
            DataBlock = ps,
            TelnetCommand = null
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Not.Zero);
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlock_MessageEncoded()
    {
        // Arrange 
        var msg = new SdcpOutboundDataMessage();

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

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