// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;

/// <summary>
/// Default implementation of a data message sorter soting the received messages by OriginalMessageId
/// </summary>
public class DefaultInboundDataMessageSorter : IInboundDataMessageSorter
{
    private readonly Dictionary<long, ISortableInboundDataMessage> _inboundDataMessages = new();

    /// <summary>
    /// The maximum number of messages in the queue before the queue is sent and then cleared
    /// </summary>
    public int MaxNumberOfMessagesInQueue { get; set; } = 5;

    /// <summary>
    /// The last incoming message ID returned correctly
    /// </summary>
    public long LastMessageId { get; set; } = long.MinValue;

    /// <summary>
    /// Add a message to the sorter and return zero, one or more messages waiting in the internal queue in the correct sort order
    /// </summary>
    /// <param name="message">Current data message</param>
    /// <returns>Zero, one or more messages waiting in the internal queue in the correct sort order</returns>
    public List<ISortableInboundDataMessage> AddMessage(ISortableInboundDataMessage message)
    {
        var result = new List<ISortableInboundDataMessage>();

        // First message
        if (LastMessageId == long.MinValue)
        {
            if (_inboundDataMessages.Count > 0)
            {
                result.AddRange(_inboundDataMessages.Values.OrderBy(x => x.OriginalMessageId));
                _inboundDataMessages.Clear();
            }

            LastMessageId = message.OriginalMessageId;
            result.Add(message);
            return result;
        }

        // Message is the expected one
        if (LastMessageId + 1 == message.OriginalMessageId)
        {
            //if (_inboundDataMessages.Count > 0)
            //{


            //}
            LastMessageId = message.OriginalMessageId;
            result.Add(message);
            return result;
        }


        // No messages waiting and ID not valid: add to queue
        if (message.OriginalMessageId > LastMessageId + 1 || message.OriginalMessageId < LastMessageId)
        {
            if (_inboundDataMessages.Count == 0)
            {
                _inboundDataMessages.Add(message.OriginalMessageId, message);
                LastMessageId = message.OriginalMessageId;
            }
            else
            {
                // Check if order fits
                _inboundDataMessages.Add(message.OriginalMessageId, message);
                LastMessageId = message.OriginalMessageId;

                var sorted = _inboundDataMessages.Values.OrderBy(x => x.OriginalMessageId).ToList();

                ISortableInboundDataMessage oldMsg = sorted.First();
                var messageId = oldMsg.OriginalMessageId;

                var fit = true;

                for (var index = 1; index < sorted.Count; index++)
                {
                    var msg = sorted[index];
                    if (msg.OriginalMessageId != messageId + 1)
                    {
                        fit = false;
                    }
                }

                if (!fit && sorted.Count < MaxNumberOfMessagesInQueue)
                {
                        return result;
                }

                result.AddRange(sorted);
                _inboundDataMessages.Clear();
            }
        }

        return result;
    }

    /// <summary>
    /// Reset the last message ID to the default value Long.MinValue
    /// </summary>
    public void ResetLastMessageId()
    {
        LastMessageId = long.MinValue;
    }
}