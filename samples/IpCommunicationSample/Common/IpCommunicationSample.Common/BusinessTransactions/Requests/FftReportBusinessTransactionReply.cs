// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace IpCommunicationSample.Common.BusinessTransactions.Requests;

/// <summary>
/// Request data for a FFT anylsis report
/// </summary>
public class FftReportBusinessTransactionReply : IBusinessTransactionReply
{
    /// <summary>
    /// The notification object to send via GRPC etc to the client
    /// </summary>
    public object? NotificationObjectToSend { get; set; }

    /// <summary>The current request data</summary>
    public IBusinessTransactionRequestData? RequestData { get; set; }

    /// <summary>
    /// Current error code. Default is 0 for no error happened
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>Current message provided by the business transaction</summary>
    public string? Message { get; set; }

    /// <summary>
    /// Current error message provided by the business transaction
    /// </summary>
    public string? ExceptionMessage { get; set; }
}