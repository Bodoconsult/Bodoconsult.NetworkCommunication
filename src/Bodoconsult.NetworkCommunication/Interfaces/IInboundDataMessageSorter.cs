// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for data message sorter instances sorting inbound data messages by its OriginalMessageId
/// </summary>
public interface IInboundDataMessageSorter
{
    /// <summary>
    /// The maximum number of messages in the queue before the queue is sent and then cleared
    /// </summary>
    int MaxNumberOfMessagesInQueue { get; set; }
    
    /// <summary>
    /// The last incoming message ID returned correctly
    /// </summary>
    long LastMessageId { get; set; }

    /// <summary>
    /// Add a message to the sorter and return zero, one or more messages waiting in the internal queue in the correct sort order
    /// </summary>
    /// <param name="message">Current data message</param>
    /// <returns>Zero, one or more messages waiting in the internal queue in the correct sort order</returns>
    List<ISortableInboundDataMessage> AddMessage(ISortableInboundDataMessage message);

    /// <summary>
    /// Reset the last message ID to the default value Long.MinValue
    /// </summary>
    void ResetLastMessageId();
}