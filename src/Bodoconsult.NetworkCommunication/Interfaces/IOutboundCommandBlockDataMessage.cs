// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based outbound messaging containing a command and a block code with data i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundCommandBlockDataMessage : IOutboundDataMessage
{
    /// <summary>
    /// Command requested
    /// </summary>
    char Command { get; }

    /// <summary>
    /// Current block code
    /// </summary>
    public byte Block { get; }
}