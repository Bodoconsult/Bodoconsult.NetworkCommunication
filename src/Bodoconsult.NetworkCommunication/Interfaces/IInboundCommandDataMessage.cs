// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging containing a command with data i.e. via TCP/IP or UDP
/// </summary>
public interface IInboundCommandDataMessage : IInboundDataMessage
{
    /// <summary>
    /// Command requested
    /// </summary>
    char Command { get; }
}