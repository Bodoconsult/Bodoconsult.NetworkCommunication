// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging containing a business transaction with data i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundBusinessTransactionDataMessage : IOutboundDataMessage
{
    /// <summary>
    /// ID of the business transaction
    /// </summary>
    int BusinessTransactionId { get; }

    /// <summary>
    /// UID of the business transaction instance
    /// </summary>
    Guid BusinessTransactionUid { get; }
}