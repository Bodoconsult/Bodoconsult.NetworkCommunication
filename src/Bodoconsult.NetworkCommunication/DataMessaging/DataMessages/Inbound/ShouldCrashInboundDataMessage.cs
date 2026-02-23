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
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    public bool AnswerWithAcknowledgement { get; set; }

    /// <summary>
    /// First plausibilty check if a received message can the expected answer to the request. 
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    public bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IList<string> errors)
    {
        return false;
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