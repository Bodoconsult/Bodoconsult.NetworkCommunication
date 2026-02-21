// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based outbound messaging i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundDataMessage
{
    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    long MessageId { get; }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    bool WaitForAcknowledgement { get; }

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