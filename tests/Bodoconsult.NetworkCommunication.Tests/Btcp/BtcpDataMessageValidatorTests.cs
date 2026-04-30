// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpDataMessageValidatorTests
{
    [Test]
    public void IsMessageValid_RawDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new RawInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_SdcpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_TncpDataMessage_ReturnsFalse()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new SdcpInboundDataMessage();

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.False);
    }

    [Test]
    public void IsMessageValid_BtcpDataMessage_ReturnsTrue()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var msg = new BtcpRequestInboundDataMessage(1, Guid.NewGuid());

        // Act  
        var result = validator.IsMessageValid(msg);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }

    [Test]
    public void IsMessageValid_BtcpDataMessageRealWorld1_ReturnsTrue()
    {
        // Arrange 
        var validator = new BtcpDataMessageValidator();

        var data = new byte[] { 0x2, 0x0, 0x32, 0x30, 0x31, 0x4, 0x35, 0x65, 0x31, 0x37, 0x34, 0x65, 0x62, 0x65, 0x2d, 0x30, 0x63, 0x64, 0x30, 0x2d, 0x34, 0x36, 0x33, 0x65, 0x2d, 0x61, 0x32, 0x64, 0x31, 0x2d, 0x39, 0x34, 0x31, 0x66, 0x66, 0x39, 0x34, 0x65, 0x65, 0x66, 0x61, 0x65, 0x4, 0x30, 0x7c, 0x7c, 0x7c, 0x78, 0x30, 0x7c, 0x7c, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);


        var msg = codec.DecodeDataMessage(data);
        Assert.That(msg.DataMessage, Is.Not.Null);

        // Act  
        var result = validator.IsMessageValid(msg.DataMessage);

        // Assert
        Assert.That(result.IsMessageValid, Is.True);
    }
}