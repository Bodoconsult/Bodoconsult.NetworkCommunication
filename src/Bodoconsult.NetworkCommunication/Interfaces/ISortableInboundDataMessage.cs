// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for sortable data messages
/// </summary>
public interface ISortableInboundDataMessage : IInboundDataMessage
{
    /// <summary>
    /// Original message ID received from the device
    /// </summary>
    long OriginalMessageId { get; set; }
}