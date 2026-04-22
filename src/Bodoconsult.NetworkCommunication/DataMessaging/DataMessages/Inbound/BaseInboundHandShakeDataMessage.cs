// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Base class for <see cref="IInboundDataMessage"/> handshake implementations
/// </summary>
public class BaseInboundHandShakeMessage: IInboundHandShakeMessage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public BaseInboundHandShakeMessage()
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