// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for a FFT analysis report
/// </summary>
public class FftReportBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Power spectrum density (PSD)
    /// </summary>
    public double[] Psd { get; set; } = [];

    /// <summary>
    /// Frequency scale
    /// </summary>
    public double[] FrequencyScale { get; set; } = [];
}