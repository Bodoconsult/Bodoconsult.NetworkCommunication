// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for starting the data collector 
/// </summary>
public class StartCollectorBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Data collection time interval in ms
    /// </summary>
    public int CollectionInterval { get; set; } = 5000;

    /// <summary>
    /// Data collection time period in ms
    /// </summary>
    public int CollectionTime { get; set; } = 1000;
}