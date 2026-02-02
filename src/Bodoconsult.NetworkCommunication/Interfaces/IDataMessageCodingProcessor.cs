
// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for encoding and decoding data messages
/// </summary>
public interface IDataMessageCodingProcessor
{
    /// <summary>
    /// All loaded message codecs
    /// </summary>
    IList<IDataMessageCodec> MessageCodecs { get; }

    /// <summary>
    /// Decode a data message
    /// </summary>
    /// <param name="data">Byte array with message data</param>
    /// <returns>Coding result with a <see cref="IDataMessage"/> instance if coding was successful</returns>
    InboundCodecResult DecodeDataMessage(Memory<byte> data);

    /// <summary>
    /// Encode handshake messages to send to device
    /// </summary>
    /// <param name="dataMessage">Data message to encode</param>
    /// <returns>A result set with the message as byte array </returns>
    OutboundCodecResult EncodeDataMessage(IDataMessage dataMessage);

}