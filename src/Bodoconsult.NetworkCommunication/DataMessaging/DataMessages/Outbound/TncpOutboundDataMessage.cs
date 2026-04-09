// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Basic implementation of <see cref="IOutboundDataMessage"/> for TNCP protocol
/// </summary>
public class TncpOutboundDataMessage : IOutboundDataMessage
{
    private Memory<byte> _rawMessageData;

    /// <summary>
    /// Default ctor
    /// </summary>
    public TncpOutboundDataMessage()
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
    /// Block code of the requesting data message this message is an answer for.
    /// Set and use this field in your business logic to build command chains.
    /// </summary>
    public byte BlockCode { get; set; }

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
    /// Delegate fired if a message was sent to an IP device
    /// </summary>
    public RaiseStopSyncExecutionDelegate? RaiseStopSyncExecutionDelegate { get; set; }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public string ToInfoString()
    {
        return $"TncpOutboundDataMessage ID {MessageId} Block {BlockCode}: {RawMessageDataClearText}";
    }

    public string ToShortInfoString()
    {
        return $"TncpOutboundDataMessage ID {MessageId} Block {BlockCode}";
    }

    /// <summary>
    /// Data block stored in the message
    /// </summary>
    public ITypedOutboundDataBlock? DataBlock { get; set; }

    /// <summary>
    /// Telnet style command
    /// </summary>
    public string? TelnetCommand { get; set; }
}