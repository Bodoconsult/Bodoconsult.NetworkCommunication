// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for a FFT analysis report
/// </summary>
public class FftReportBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Array with a list chunks are rendered to 9 bytes each: first byte is the channel and the next 8 bytes are chunk data
    /// </summary>
    public byte[] Bytes { get; set; } = [];
}