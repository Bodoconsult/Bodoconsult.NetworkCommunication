// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Base class for <see cref="IDataMessageCodec"/> implementations
/// </summary>
public abstract class BaseDataMessageCodec: IDataMessageCodec
{
    /// <summary>
    /// Expected minimum length of the message or 0
    /// </summary>
    public int ExpectedMinimumLength { get; protected set; }

    /// <summary>
    /// Expected maximum length of the message or 0
    /// </summary>
    public int ExpectedMaximumLength { get; protected set; }

    /// <summary>
    /// Decode a data message to an <see cref="IInboundDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public virtual InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Encodes a message to a byte array to send to receiver
    /// </summary>
    /// <param name="message">Data message to send</param>
    /// <returns>Codex result with a byte array as optimized <see cref="ReadOnlyMemory{T}"/> to send</returns>
    public virtual OutboundCodecResult EncodeDataMessage(IOutboundDataMessage message)
    {
        throw new NotSupportedException();
    }

    protected InboundCodecResult CheckExpectedLengths(int length)
    {
        var result = new InboundCodecResult();

        if (ExpectedMinimumLength!=0 && length < ExpectedMinimumLength)
        {
            result.ErrorCode = 1;
            return result;
        }

        if (ExpectedMaximumLength!=0 && length > ExpectedMaximumLength)
        {
            result.ErrorCode = 2;
        }

        return result;
    }
}