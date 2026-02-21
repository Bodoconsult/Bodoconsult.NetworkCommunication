// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Base class for <see cref="IOutboundDataMessage"/> handshake implementations
/// </summary>
public class BaseOutboundHandShakeDataMessage: IOutboundDataMessage
{
    public BaseOutboundHandShakeDataMessage()
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
    }

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get;  }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    public bool WaitForAcknowledgement => false;

    /// <summary>
    /// Current raw message data as byte array
    /// </summary>
    public Memory<byte> RawMessageData { get; set; }

    /// <summary>
    /// Current raw message data as clear text
    /// </summary>
    public string RawMessageDataClearText { get; set; }

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

}