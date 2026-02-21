// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="IInboundDataMessage"/> for SDCP protocol
/// </summary>
public class SdcpInboundDataMessage: IInboundDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SdcpInboundDataMessage()
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
    }

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get; }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    public bool WaitForAcknowledgement { get; set; }

    /// <summary>
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    public bool AnswerWithAcknowledgement { get; set; }

    /// <summary>
    /// Current raw message data as byte array
    /// </summary>
    public Memory<byte> RawMessageData
    {
        get => _rawMessageData;
        set
        {
            _rawMessageData = value;
            RawMessageDataClearText = DataMessageHelper.GetStringFromArrayCsharpStyle(_rawMessageData);
        }
    }

    /// <summary>
    /// Current raw message data as clear text
    /// </summary>
    public string RawMessageDataClearText { get; set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToInfoString()
    {
        return $"SdcpInboundDataMessage ID {MessageId} {RawMessageDataClearText}";
    }

    public string ToShortInfoString()
    {
        return $"SdcpInboundDataMessage ID {MessageId}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public IDataBlock DataBlock { get; set; }
}