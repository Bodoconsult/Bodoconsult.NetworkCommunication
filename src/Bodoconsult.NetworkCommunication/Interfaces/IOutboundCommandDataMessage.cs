// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based outbound messaging containing a command with data i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundCommandDataMessage : IOutboundDataMessage
{
    /// <summary>
    /// Command requested
    /// </summary>
    char Command { get; }
}