// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode raw data messages
/// </summary>
public class RawDataMessageCodec : BaseDataMessageCodec
{


    public RawDataMessageCodec()
    {
        ExpectedMinimumLength = 1;
        ExpectedMaximumLength = int.MaxValue;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        var result = new InboundCodecResult
        {
            DataMessage =new RawDataMessage
            {
                RawMessageData = data
            },
            ErrorCode = 0
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
        if (message is not RawDataMessage rm)
        {
            result.ErrorMessage = "RawDataMessage required for RawDataMessageCodec";
            result.ErrorCode= 1;
            return result;
        }

        if (message.RawMessageData.Length==0)
        {
            result.ErrorMessage = "No data provided for message";
            result.ErrorCode= 1;
            return result;
        }

        message.RawMessageData = rm.RawMessageData;
        return result;
    }
}