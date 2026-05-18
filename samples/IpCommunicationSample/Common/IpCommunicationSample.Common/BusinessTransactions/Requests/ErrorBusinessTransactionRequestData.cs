// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
    /// Request data for reporting an device error to 
    /// </summary>
    public class ErrorBusinessTransactionRequestData : BaseBusinessTransactionRequestData
{
    /// <summary>
    /// Telent command
    /// </summary>
    public string? TelnetCommand { get; set; }

    /// <summary>
    /// Additional info for the telnet command
    /// </summary>
    public string? TelnetAdditionalInfo { get; set; }
}