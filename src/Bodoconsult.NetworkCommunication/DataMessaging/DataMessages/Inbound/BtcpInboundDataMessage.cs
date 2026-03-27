// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="IInboundBusinessTransactionDataMessage"/> for BTCP protocol
/// </summary>
public class BtcpInboundDataMessage : IInboundBusinessTransactionDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionId">ID of the business transaction</param>
    /// <param name="businessTransactionUid">UID of the business transaction instance</param>
    public BtcpInboundDataMessage(int businessTransactionId, Guid businessTransactionUid)
    {
        MessageId = DateTime.Now.ToFileTimeUtc();
        BusinessTransactionId = businessTransactionId;
        BusinessTransactionUid = businessTransactionUid;
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
    /// Is the message a request for running a business transaction? True = request for running a business transaction, false reply on a request to run a business transaction
    /// </summary>
    public bool IsRequest { get; set; }

    /// <summary>
    /// First plausibilty check if a received message can be the expected answer to the request. 
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    public bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IList<string> errors)
    {
        if (sentMessage is not IOutboundBusinessTransactionDataMessage btm)
        {
            return false;
        }

        return btm.BusinessTransactionId == BusinessTransactionId;
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
        return $"BtcpInboundDataMessage ID {MessageId} BT {BusinessTransactionId} {RawMessageDataClearText}";
    }

    public string ToShortInfoString()
    {
        return $"BtcpInboundDataMessage ID {MessageId} BT {BusinessTransactionId}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public ITypedInboundDataBlock? DataBlock { get; set; }

    /// <summary>
    /// ID of the business transaction
    /// </summary>
    public int BusinessTransactionId { get;  }

    /// <summary>
    /// UID of the business transaction instance
    /// </summary>
    public Guid BusinessTransactionUid { get; }
}