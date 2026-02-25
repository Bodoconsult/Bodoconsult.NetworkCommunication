// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.EventCounters;
using Bodoconsult.App.Interfaces;
using System.Diagnostics.Tracing;

namespace Bodoconsult.NetworkCommunication.EventSources;

/// <summary>
/// Provider for device comm general event counters (DCL = device communication layer)
/// </summary>
public class DataMessagingEventSourceProvider : IEventSourceProvider
{
    public const string DclSentDataMessageCount = "DCL.SentDataMessage.Count";

    public const string DclSentDataMessageBytes = "DCL.SentDataMessage.Bytes";

    public const string DclReceivedDataMessageCount = "DCL.ReceivedDataMessage.Count";

    public const string DclReceivedDataMessageBytes = "DCL.ReceivedDataMessage.Bytes";

    /// <summary>
    /// Add <see cref="EventCounter"/> to the event source
    /// </summary>
    /// <param name="eventSource">Current event source</param>
    public void AddEventCounters(AppApmEventSource eventSource)
    {
        CreateTclSendTowerMessageBytesEventCounter(eventSource);
        CreateTclReceivedTowerMessageBytesEventCounter(eventSource);
    }

    private static void CreateTclSendTowerMessageBytesEventCounter(AppApmEventSource eventSource)
    {
        if (eventSource.EventCounters.ContainsKey(DclSentDataMessageBytes))
        {
            return;
        }

        var ec = new EventCounter(DclSentDataMessageBytes, eventSource)
        {
            DisplayName = "Bytes sent in tower messages",
            DisplayUnits = "bytes"
        };

        eventSource.EventCounters.Add(DclSentDataMessageBytes, ec);
    }

    private static void CreateTclReceivedTowerMessageBytesEventCounter(AppApmEventSource eventSource)
    {
        if (eventSource.EventCounters.ContainsKey(DclReceivedDataMessageBytes))
        {
            return;
        }

        var ec = new EventCounter(DclReceivedDataMessageBytes, eventSource)
        {
            DisplayName = "Bytes received in tower messages",
            DisplayUnits = "bytes"
        };

        eventSource.EventCounters.Add(DclReceivedDataMessageBytes, ec);
    }


    /// <summary>
    /// Add <see cref="IncrementingEventCounter"/> to the event source
    /// </summary>
    /// <param name="eventSource">Current event source</param>
    public void AddIncrementingEventCounters(AppApmEventSource eventSource)
    {
        CreateTclSendTowerMessageCountEventCounter(eventSource);
        CreateTclReceivedTowerMessageCountEventCounter(eventSource);
    }

    private static void CreateTclSendTowerMessageCountEventCounter(AppApmEventSource eventSource)
    {
        if (eventSource.IncrementingEventCounters.ContainsKey(DclSentDataMessageCount))
        {
            return;
        }

        var ec = new IncrementingEventCounter(DclSentDataMessageCount, eventSource)
        {
            DisplayName = "Data messages sent",
            DisplayUnits = "messages sent"
        };

        eventSource.IncrementingEventCounters.Add(DclSentDataMessageCount, ec);
    }

    private static void CreateTclReceivedTowerMessageCountEventCounter(AppApmEventSource eventSource)
    {
        if (eventSource.IncrementingEventCounters.ContainsKey(DclReceivedDataMessageCount))
        {
            return;
        }

        var ec = new IncrementingEventCounter(DclReceivedDataMessageCount, eventSource)
        {
            DisplayName = "Data messages received",
            DisplayUnits = "messages sent"
        };

        eventSource.IncrementingEventCounters.Add(DclReceivedDataMessageCount, ec);
    }

    /// <summary>
    /// Add e<see cref="PollingCounter"/> to the event source
    /// </summary>
    /// <param name="eventSource">Current event source</param>
    public void AddPollingCounters(AppApmEventSource eventSource)
    {
        // Do nothing
    }

    /// <summary>
    /// Add <see cref="IncrementingPollingCounter"/> to the event source
    /// </summary>
    /// <param name="eventSource">Current event source</param>
    public void AddIncrementingPollingCounters(AppApmEventSource eventSource)
    {
        // Do nothing
    }
}
