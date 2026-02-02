// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode handshake messages for SDCP protocol
/// </summary>
public class SdcpHandshakeMessageCodec : BaseDataMessageCodec
{

    public SdcpHandshakeMessageCodec()
    {
        ExpectedMinimumLength = 1;
        ExpectedMaximumLength = 1;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        var result = CheckExpectedLengths(data.Length);

        if (result.ErrorCode != 0)
        {
            return result;
        }

        if (!DeviceCommunicationBasics.HandshakeMessageStartTokens.Contains(data.Span[0]))
        {
            result.ErrorCode = 3;
            result.ErrorMessage = $"First char {data.Span[0]:X2} is not an allowed handshake char!";
            return result;
        }

        result.DataMessage = new HandshakeMessage(MessageTypeEnum.Received)
        {
            HandshakeMessageType = data.Span[0],
            RawMessageData = data.ToArray()
        };
        return result;
    }

    /// <summary>
    /// Encodes a message to a byte array to send to receiver
    /// </summary>
    /// <param name="message">Data message to send</param>
    /// <returns>Byte array as optimized <see cref="ReadOnlyMemory{T}"/> to send</returns>
    public override OutboundCodecResult EncodeDataMessage(IDataMessage message)
    {
        var result = new OutboundCodecResult();

        if (message is not HandshakeMessage hMessage)
        {
            result.ErrorMessage = "HandshakeMessage required for HandshakeMessageCodec";
            result.ErrorCode = 1;
            return result;
        }

        message.RawMessageData = new[] { hMessage.HandshakeMessageType};
        return result;
    }
}