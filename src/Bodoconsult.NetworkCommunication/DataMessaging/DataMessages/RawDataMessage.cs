// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Sends a raw byte data message to the device. Intended mainly for outgoing message to send
/// </summary>
public class RawDataMessage : IDataMessage
{

    private Memory<byte> _rawMessageData;

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get; }= DateTime.Now.Ticks;

    /// <summary>
    /// The message type of the message
    /// </summary>
    public MessageTypeEnum MessageType => MessageTypeEnum.Sent;

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    public bool WaitForAcknowledgement => false;

    /// <summary>
    /// Should a acknowledgement be sent if the message is received
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
    public string RawMessageDataClearText { get; private set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToInfoString()
    {
        return $"RawDataMessage {MessageId} Length:{RawMessageData.Length} Data:{DataMessageHelper.ByteArrayToString(RawMessageData)}";
    }

    /// <summary>
    /// Create an short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToShortInfoString()
    {
        return $"RawDataMessage {MessageId} Length:{RawMessageData.Length}";
    }
}