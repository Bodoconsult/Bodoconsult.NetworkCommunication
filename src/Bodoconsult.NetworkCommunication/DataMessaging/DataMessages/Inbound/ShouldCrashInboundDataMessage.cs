// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Dummy message for tests letting the converter fail for SDCP and EDCP
/// </summary>
public class ShouldCrashInboundDataMessage : IInboundDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get; } = DateTime.Now.Ticks;

    /// <summary>
    /// The message type of the message
    /// </summary>
    public MessageTypeEnum MessageType { get; set; }

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
    public string RawMessageDataClearText { get; private set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToInfoString()
    {
        return $"ShouldCrashDataMessage {MessageId} Length:{RawMessageData.Length} Data:{RawMessageDataClearText}";
    }

    /// <summary>
    /// Create an short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToShortInfoString()
    {
        return $"ShouldCrashDataMessage {MessageId} Length:{RawMessageData.Length}";
    }
}