// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
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

    // 

    [Test]
    public void DecodeDataMessage_ValidInputRealWorld1_MessageDecoded()
    {
        // Arrange 
        const string cmd = "<BEGIN>set,stream,number,1";

        var msg = new byte[]{ 0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c, 0x31, 0xa };

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
    public void DecodeDataMessage_ValidInput_MessageDecoded()
    {
        // Arrange 
        var cmd = "log,charstat";

        var msg = Encoding.UTF8.GetBytes($"{cmd}\n");

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
    public void DecodeDataMessage_ValidResponseCommand_MessageDecoded()
    {
        // Arrange 
        var cmd = "log,charstat";
        var response = $"<BEGIN>{cmd}\n<END>";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandRealWorld1_MessageDecoded()
    {
        // Arrange 
        var cmd = "set,stream,order,1,2,3,4";
        var response = $"<BEGIN>{cmd}\n<END>\n";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.Null);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandRealWorld2_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[]
        {
            0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d,
            0x2c, 0x6f, 0x72, 0x64, 0x65, 0x72, 0x2c, 0x31, 0x2c, 0x34, 0xa, 0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e,
            0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6f, 0x72, 0x64, 0x65, 0x72,
            0x2c, 0x31, 0x2c, 0x34, 0xa
        };
            

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

            Debug.Print($"{tncpMsg.TelnetCommand}");
            Debug.Print($"{tncpMsg.TelnetAdditionalInfo}");

            Assert.That(tncpMsg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo("<BEGIN>set,stream,order,1,4"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.Null);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandRealWorld3_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[]
        {
            0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6f, 0x72, 0x64, 0x65, 0x72, 0x2c, 0x33, 0xa, 0x3c, 0x45, 0x4e, 0x44, 0x3e, 0xa
        };

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

            Debug.Print($"{tncpMsg.TelnetCommand}");
            Debug.Print($"{tncpMsg.TelnetAdditionalInfo}");

            Assert.That(tncpMsg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo("<BEGIN>set,stream,order,3"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.Null);
        }
    }

   
    [Test]
    public void DecodeDataMessage_ValidResponseCommandRealWorld4_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[]
            {
                0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x68, 0x6f, 0x77, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x63, 0x6f, 0x6e, 0x66, 0x69, 0x67, 0xa, 0x3c, 0x43, 0x4f, 0x4e, 0x46, 0x49, 0x47, 0x3e, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x30, 0x30, 0x78, 0x63, 0x30, 0x78, 0x66, 0xa, 0x3c, 0x45, 0x4e, 0x44, 0x3e, 0xa
            };

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

            Debug.Print($"{tncpMsg.TelnetCommand}");
            Debug.Print($"{tncpMsg.TelnetAdditionalInfo}");

            Assert.That(tncpMsg.RawMessageData.Length, Is.Not.Zero);
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo("<BEGIN>show,streamconfig"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.Not.Null);
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
    public void DecodeDataMessage_ValidResponseCommandWithError_MessageDecoded()
    {
        // Arrange 
        var cmd = "log,charstat";
        var error = "<ERROR>Invalid command foo";
        var response = $"<BEGIN>{cmd}\n{error}\n<END>";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.EqualTo(error));
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandWithConfig1_MessageDecoded()
    {
        // Arrange 
        var cmd = "show,streamconfig";
        var config = "<CONFIG>0x00x10x20x30xC0xF";
        var response = $"<BEGIN>{cmd}\n{config}\n<END>";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.EqualTo(config));
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandWithConfig2_MessageDecoded()
    {
        // Arrange 
        var cmd = "show,streamconfig";
        var config = "<CONFIG>0x00x00x00xC0xF";
        var response = $"<BEGIN>{cmd}\n{config}\n<END>";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.EqualTo(config));
        }
    }

    [Test]
    public void DecodeDataMessage_ValidResponseCommandWithConfig3_MessageDecoded()
    {
        // Arrange 
        var cmd = "show,streamconfig";
        var config = "<CONFIG>0x00xC0xF";
        var response = $"<BEGIN>{cmd}\n{config}\n<END>";

        var msg = Encoding.UTF8.GetBytes(response);

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
            Assert.That(tncpMsg.TelnetCommand, Is.EqualTo($"<BEGIN>{cmd}"));
            Assert.That(tncpMsg.TelnetAdditionalInfo, Is.EqualTo(config));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidDataBlockInput_MessageEncoded()
    {
        // Arrange 
        const string cmd = "<BEGIN>set,stream,number,1";

        var data = Encoding.UTF8.GetBytes(cmd);

        var dataBlock = new BasicOutboundDatablock
        {
            Data = new Memory<byte>(data),
            DataBlockType = 'x'
        };

        var msg = new TncpOutboundDataMessage
        {
            DataBlock = dataBlock,
            //TelnetCommand = cmd
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
                Is.EqualTo(DeviceCommunicationBasics.Lf));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidCommand_MessageEncoded()
    {
        // Arrange 
        const string cmd = "<BEGIN>set,stream,number,1";

        var msg = new TncpOutboundDataMessage
        {
            //DataBlock = dataBlock,
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
                Is.EqualTo(DeviceCommunicationBasics.Lf));
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
                Is.EqualTo(DeviceCommunicationBasics.Lf));
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