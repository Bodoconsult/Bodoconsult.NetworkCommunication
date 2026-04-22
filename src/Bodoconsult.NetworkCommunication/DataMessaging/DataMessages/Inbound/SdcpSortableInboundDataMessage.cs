// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="ISortableInboundDataMessage"/> for SDCP protocol
/// </summary>
public class SdcpSortableInboundDataMessage : ISortableInboundDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SdcpSortableInboundDataMessage()
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
    }

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get; }

    /// <summary>
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    public bool AnswerWithAcknowledgement { get; set; }
    
    /// <summary>
    /// First plausibilty check if a received message can be the expected answer to the request. 
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    public bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IList<string> errors)
    {
        return sentMessage is SdcpOutboundDataMessage;
    }

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
    public string? RawMessageDataClearText { get; set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToInfoString()
    {
        return $"SdcpInboundDataMessage ID {MessageId} {RawMessageDataClearText}";
    }

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToShortInfoString()
    {
        return $"SdcpInboundDataMessage ID {MessageId}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public ITypedInboundDataBlock? DataBlock { get; set; }

    /// <summary>
    /// Original message ID received from the device
    /// </summary>
    public long OriginalMessageId { get; set; }
}