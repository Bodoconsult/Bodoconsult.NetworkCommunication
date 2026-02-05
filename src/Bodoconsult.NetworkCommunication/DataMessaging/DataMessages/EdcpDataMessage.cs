// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="IDataMessage"/> for EDCP protocol
/// </summary>
public class EdcpDataMessage : IDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpDataMessage()
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
    }

    /// <summary>
    /// A unique ID to identify the message
    /// </summary>
    public long MessageId { get; }

    /// <summary>
    /// The message type of the message
    /// </summary>
    public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Received;

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    public bool WaitForAcknowledgement { get; set; }

    /// <summary>
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    public bool AnswerWithAcknowledgement { get; set; }

    /// <summary>
    /// Current block code of this message
    /// </summary>
    public byte BlockCode { get; set; }

    /// <summary>
    /// Block code of the requesting data message this message is an answer for.
    /// Set and use this field in your business logic to build command chains.
    /// </summary>
    public byte RequestBlockCode { get; set; }

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
        return $"EdcpDataMessage ID {MessageId} {MessageType.ToString()} Block {BlockCode}: {RawMessageDataClearText}";
    }

    public string ToShortInfoString()
    {
        return $"EdcpDataMessage ID {MessageId} {MessageType.ToString()} Block {BlockCode}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public IDataBlock DataBlock { get; set; }
}