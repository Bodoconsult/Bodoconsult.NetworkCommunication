// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for byte based outbound messaging i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundMessage
{
    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    long MessageId { get; }

    /// <summary>
    /// Current raw message data as byte array
    /// </summary>
    Memory<byte> RawMessageData { get; set; }

    /// <summary>
    /// Current raw message data as clear text
    /// </summary>
    string? RawMessageDataClearText { get; }

    /// <summary>
    /// Delegate fired if a message was sent to an IP device
    /// </summary>
    RaiseStopSyncExecutionDelegate? RaiseStopSyncExecutionDelegate { get; set; }

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