// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for reporting an device error to 
/// </summary>
public class LoadStreamingConfigBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Current streaming config
    /// </summary>
    public byte[] Config { get; set; } = [];
}