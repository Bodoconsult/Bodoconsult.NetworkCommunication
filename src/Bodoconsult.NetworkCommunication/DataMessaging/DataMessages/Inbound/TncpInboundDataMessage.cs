// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="IInboundBusinessTransactionDataMessage"/> for TNCP protocol
/// </summary>
public class TncpInboundDataMessage : IInboundDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    public TncpInboundDataMessage()
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
        if (sentMessage is not TncpOutboundDataMessage)
        {
            return false;
        }

        return true;
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
        return $"BtcpInboundDataMessage ID {MessageId} Command {TelnetCommand} {RawMessageDataClearText}";
    }

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToShortInfoString()
    {
        return $"BtcpInboundDataMessage ID {MessageId} Command {TelnetCommand}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public ITypedInboundDataBlock? DataBlock { get; set; }

    /// <summary>
    /// Telnet command to send
    /// </summary>
    public string? TelnetCommand { get; set; }
}