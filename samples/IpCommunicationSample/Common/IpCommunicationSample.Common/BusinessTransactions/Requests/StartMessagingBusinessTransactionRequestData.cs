// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for starting device communication
/// </summary>
public class StartMessagingBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Is the request for snapshot (false) or streaming (true)
    /// </summary>
    public bool Snapshot { get; set; }

    /// <summary>
    /// Use a software defined snapshot
    /// </summary>
    public bool UseSoftwareSnapshot { get; set; }

    /// <summary>
    /// Use a software defined snapshot
    /// </summary>
    public bool IsDataLoggingActivated { get; set; }

    /// <summary>
    /// Is data presentation as chart activated?
    /// </summary>
    public bool IsChartActivated { get; set; }

    /// <summary>
    /// Show channel 1?
    /// </summary>
    public bool Channel1 { get; set; }

    /// <summary>
    /// Show channel 2?
    /// </summary>
    public bool Channel2 { get; set; }

    /// <summary>
    /// Show channel 3?
    /// </summary>
    public bool Channel3 { get; set; }

    /// <summary>
    /// Show channel 4?
    /// </summary>
    public bool Channel4 { get; set; }

    /// <summary>
    /// Data collection time interval in ms
    /// </summary>
    public int CollectionInterval { get; set; } = 5000;

    /// <summary>
    /// Data collection time period in ms
    /// </summary>
    public int CollectionTime { get; set; } = 1000;
}