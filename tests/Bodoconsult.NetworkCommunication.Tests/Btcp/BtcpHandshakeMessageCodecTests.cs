// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpHandshakeMessageCodecTests
{
    [Test]
    public void DecodeDataMessage_ValidHandshake_ReturnsHandshakeMessage()
    {
        // Arrange 
        var message = new byte[] { 0x6 };

        var codec = new BtcpHandshakeMessageCodec();

        // Act  
        var result = codec.DecodeDataMessage(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DataMessage, Is.Not.Null);

        ArgumentNullException.ThrowIfNull(result.DataMessage);

        Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(InboundHandshakeMessage)));
        var msg = (InboundHandshakeMessage)result.DataMessage;
        Assert.That(msg.HandshakeMessageType, Is.EqualTo(message[0]));
    }

    [Test]
    public void EncodeDataMessage_ValidHandshakeMessage_CreatesHandshake()
    {
        // Arrange 
        var message = new OutboundHandshakeMessage
        {
            HandshakeMessageType = 0x6
        };

        var codec = new BtcpHandshakeMessageCodec();

        // Act  
        var result = codec.EncodeDataMessage(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Zero);

        Assert.That(message.RawMessageData.Length, Is.EqualTo(1));
        Assert.That(message.RawMessageData[..1].Span[0], Is.EqualTo(message.HandshakeMessageType));
    }
}