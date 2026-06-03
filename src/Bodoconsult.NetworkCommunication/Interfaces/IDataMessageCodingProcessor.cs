
// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for encoding and decoding data messages
/// </summary>
public interface IDataMessageCodingProcessor
{
    /// <summary>
    /// Current logger
    /// </summary>
    IAppLoggerProxy Logger { get; }

    /// <summary>
    /// Should an acknowledgement be sent if a request message is received
    /// </summary>
    bool AnswerWithAcknowledgement { get; }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the request message
    /// </summary>
    bool WaitForAcknowledgement { get; }

    /// <summary>
    /// All loaded message codecs
    /// </summary>
    IList<IDataMessageCodec> MessageCodecs { get; }

    /// <summary>
    /// Add a message codex
    /// </summary>
    /// <param name="codec">Codec to add</param>
    void AddMessageCodec (IDataMessageCodec codec);

    /// <summary>
    /// Decode a data message
    /// </summary>
    /// <param name="data">Byte array with message data</param>
    /// <returns>Coding result with a <see cref="IInboundDataMessage"/> instance if coding was successful</returns>
    InboundCodecResult DecodeDataMessage(Memory<byte> data);

    /// <summary>
    /// Encode handshake messages to send to device
    /// </summary>
    /// <param name="dataMessage">Data message to encode</param>
    /// <returns>A result set with the message as byte array </returns>
    OutboundCodecResult EncodeDataMessage(IOutboundMessage dataMessage);

}