// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte stream based messaging i.e. via TCP/IP
/// </summary>
public interface IDataMessage
{

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    long MessageId { get; }

    /// <summary>
    /// The message type of the message
    /// </summary>
    MessageTypeEnum MessageType { get; }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    bool WaitForAcknowledgement { get; }

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