// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;

/// <summary>
/// Default implementation of a data message sorter soting the received messages by OriginalMessageId
/// </summary>
public class DefaultInboundDataMessageSorter : IInboundDataMessageSorter
{
    private readonly SortedList<ulong, ISortableInboundDataMessage> _inboundDataMessages = new();
    private bool _isNoStart;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current logger</param>
    public DefaultInboundDataMessageSorter(IAppLoggerProxy logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Current logger
    /// </summary>
    public IAppLoggerProxy Logger { get; }

    /// <summary>
    /// The maximum number of messages in the queue before the queue is sent and then cleared
    /// </summary>
    public int MaxNumberOfMessagesInQueue { get; set; } = 100;

    /// <summary>
    /// The last incoming message ID returned correctly
    /// </summary>
    public ulong LastMessageId { get; set; } = ulong.MinValue;

    /// <summary>
    /// Add a message to the sorter and return zero, one or more messages waiting in the internal queue in the correct sort order
    /// </summary>
    /// <param name="message">Current data message</param>
    /// <returns>Zero, one or more messages waiting in the internal queue in the correct sort order</returns>
    public List<ISortableInboundDataMessage> AddMessage(ISortableInboundDataMessage message)
    {
        var result = new List<ISortableInboundDataMessage>();

        //if (message.OriginalMessageId == 2)
        //{
        //    Debug.Print("Yes");
        //}

        // First message
        if (!_isNoStart)
        {
            if (_inboundDataMessages.Count > 0)
            {
                result.AddRange(_inboundDataMessages.Values);
                _inboundDataMessages.Clear();
            }

            result.Add(message);
            LastMessageId = message.OriginalMessageId;
            _isNoStart = true;
            return result;
        }

        // Shortcut: Next message and no message waiting
        if (_inboundDataMessages.Count == 0 && message.OriginalMessageId == LastMessageId + 1)
        {
            result.Add(message);
            LastMessageId = message.OriginalMessageId;
            return result;
        }

        if (_inboundDataMessages.TryAdd(message.OriginalMessageId, message))
        {
            //Debug.Print($"$New value: {message.OriginalMessageId}");
            CheckMessagesInQueue(result);
            //return result;
        }

        return result;

    }

    private void CheckMessagesInQueue(List<ISortableInboundDataMessage> result)
    {
        var sorted = _inboundDataMessages.Take(10).ToList();

        //var oldMsg = sorted.First();
        var messageId = LastMessageId;

        var fit = true;
        var lastFitIndex = -1;

        for (var index = 0; index < sorted.Count; index++)
        {
            var msg = sorted[index];
            if (msg.Value.OriginalMessageId != messageId + 1)
            {
                fit = false;
                break;
            }

            lastFitIndex = index;
            messageId = msg.Value.OriginalMessageId;
        }

        // Full fit
        if (fit)
        {
            foreach (var msg in sorted)
            {
                if (msg.Value.OriginalMessageId >= LastMessageId)
                {
                    result.Add(msg.Value);
                }
                _inboundDataMessages.Remove(msg.Key);
            }

            LastMessageId = result.Last().OriginalMessageId;
            return;
        }

        // No fit but some are fitting
        if (lastFitIndex >= 0)
        {
            foreach (var msg in sorted.GetRange(0, lastFitIndex + 1))
            {
                if (msg.Value.OriginalMessageId > LastMessageId)
                {
                    result.Add(msg.Value);
                }
                _inboundDataMessages.Remove(msg.Key);
            }

            LastMessageId = result.Last().OriginalMessageId;
            return;
        }

        // Queue has some free places
        if (_inboundDataMessages.Count <= MaxNumberOfMessagesInQueue)
        {
            return;
        }

        // No more free places: clear the queue
        foreach (var msg in _inboundDataMessages.ToList())
        {
            if (msg.Value.OriginalMessageId > LastMessageId)
            {
                result.Add(msg.Value);
            }
            _inboundDataMessages.Remove(msg.Key);
        }

        if (result.Count == 0)
        {
            return;
        }
        LastMessageId = result.Last().OriginalMessageId;
    }

    /// <summary>
    /// Reset the last message ID to the default value Long.MinValue
    /// </summary>
    public void ResetLastMessageId()
    {
        LastMessageId = ulong.MinValue;
    }
}