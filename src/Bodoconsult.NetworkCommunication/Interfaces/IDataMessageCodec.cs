
// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for codec implementation for encoding and decoding messages to and from byte data
/// </summary>
public interface IDataMessageCodec
{
    /// <summary>
    /// Expected minimum length of the message or 0
    /// </summary>
    int ExpectedMinimumLength { get;  }

    /// <summary>
    /// Expected maximum length of the message or 0
    /// </summary>
    int ExpectedMaximumLength { get;  }

    /// <summary>
    /// Decode a data message to an <see cref="IDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    InboundCodecResult DecodeDataMessage(Memory<byte> data);

    /// <summary>
    /// Encodes a message to a byte array to send to receiver
    /// </summary>
    /// <param name="message">Data message to send</param>
    /// <returns>Codex result with a byte array as optimized <see cref="ReadOnlyMemory{T}"/> to send</returns>
    OutboundCodecResult EncodeDataMessage(IDataMessage message);

}