// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging containing a business transaction with data i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundBusinessTransactionDataMessage : IOutboundDataMessage
{
    /// <summary>
    /// ID of the business transaction
    /// </summary>
    public int BusinessTransactionId { get; }
}