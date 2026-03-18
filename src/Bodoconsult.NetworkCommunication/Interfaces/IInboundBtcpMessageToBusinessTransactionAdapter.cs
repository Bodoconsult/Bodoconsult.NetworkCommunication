// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Adapter to bind receiving data messages to business transactions
/// </summary>
public interface IInboundBtcpMessageToBusinessTransactionAdapter: IInboundDataMessageToBusinessTransactionAdapter
{
    /// <summary>
    /// Handle an async received BTCP message
    /// </summary>
    HandleAsyncBtcpMessageDelegate? HandleAsyncBtcpMessageDelegate { get; set; }
}