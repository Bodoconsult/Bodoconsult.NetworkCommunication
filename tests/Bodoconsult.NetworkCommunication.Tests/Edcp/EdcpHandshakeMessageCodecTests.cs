// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Edcp;

[TestFixture]
internal class EdcpHandshakeMessageCodecTests
{
    [Test]
    public void DecodeDataMessage_ValidHandshake_ReturnsHandshakeMessage()
    {
        // Arrange 
        var message = new byte[] { 0x6, 0x1 };

        var codec = new EdcpHandshakeMessageCodec();

        // Act  
        var result = codec.DecodeDataMessage(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DataMessage, Is.Not.Null);

        ArgumentNullException.ThrowIfNull(result.DataMessage);

        Assert.That(result.DataMessage.GetType(), Is.EqualTo(typeof(EdcpInboundHandshakeMessage)));
        var msg = (EdcpInboundHandshakeMessage)result.DataMessage;
        Assert.That(msg.HandshakeMessageType, Is.EqualTo(message[0]));
        Assert.That(msg.BlockCode, Is.EqualTo(message[1]));
    }

    [Test]
    public void EncodeDataMessage_ValidHandshakeMessage_CreatesHandshake()
    {
        // Arrange 
        var message = new EdcpOutboundHandshakeMessage
        {
            HandshakeMessageType = 0x6,
            BlockCode = 0x1
        };

        var codec = new EdcpHandshakeMessageCodec();

        // Act  
        var result = codec.EncodeDataMessage(message);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ErrorCode, Is.Zero);

        Assert.That(message.RawMessageData.Length, Is.EqualTo(2));
        Assert.That(message.RawMessageData.Slice(0, 1).Span[0], Is.EqualTo(message.HandshakeMessageType));
        Assert.That(message.RawMessageData.Slice(1, 1).Span[0], Is.EqualTo(message.BlockCode));
    }
}