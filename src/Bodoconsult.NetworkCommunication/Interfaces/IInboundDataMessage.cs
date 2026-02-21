// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging i.e. via TCP/IP or UDP
/// </summary>
public interface IInboundDataMessage
{
    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    long MessageId { get; }

    /// <summary>
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    bool AnswerWithAcknowledgement { get; }

    /// <summary>
    /// Current raw message data as byte array
    /// </summary>
    Memory<byte> RawMessageData { get; set; }

    /// <summary>
    /// Current raw message data as clear text
    /// </summary>
    string RawMessageDataClearText { get; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    string ToInfoString();

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    string ToShortInfoString();
}