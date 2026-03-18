// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging containing a business transaction with data i.e. via TCP/IP or UDP
/// </summary>
public interface IInboundBusinessTransactionDataMessage : IInboundDataMessage
{
    /// <summary>
    /// ID of the business transaction
    /// </summary>
    public int BusinessTransactionId { get; }

    /// <summary>
    /// Is the message a request for running a business transaction? True = request for running a business transaction, false reply on a request to run a business transaction
    /// </summary>
    public bool IsRequest { get; set; }
}