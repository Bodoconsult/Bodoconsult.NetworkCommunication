// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Base class for <see cref="IOutboundDataMessage"/> handshake implementations
/// </summary>
public abstract class BaseOutboundHandShakeMessage: IOutboundHandShakeMessage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseOutboundHandShakeMessage()
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
    }

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get;  }

    /// <summary>
    /// Current raw message data as byte array
    /// </summary>
    public Memory<byte> RawMessageData { get; set; }

    /// <summary>
    /// Current raw message data as clear text
    /// </summary>
    public string? RawMessageDataClearText { get; set; }

    /// <summary>
    /// Delegate fired if a message was sent to an IP device
    /// </summary>
    public RaiseStopSyncExecutionDelegate? RaiseStopSyncExecutionDelegate { get; set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public virtual string ToInfoString()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public virtual string ToShortInfoString()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Type of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { get; set; }
}