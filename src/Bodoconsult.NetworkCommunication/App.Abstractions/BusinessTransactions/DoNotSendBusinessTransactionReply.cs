// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.BusinessTransactions;

/// <summary>
/// A reply not send to the consumer of the reply
/// </summary>
public class DoNotSendBusinessTransactionReply : IBusinessTransactionReply
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