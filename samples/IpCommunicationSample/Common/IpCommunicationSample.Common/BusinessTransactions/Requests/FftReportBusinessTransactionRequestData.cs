// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for a FFT analysis report
/// </summary>
public class FftReportBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{

}

/// <summary>
/// Request data for starting device communication
/// </summary>
public class StartMessagingReportBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Is the request for snapshot (false) or streaming (true)
    /// </summary>
    public bool Snapshot { get; set; }

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
}