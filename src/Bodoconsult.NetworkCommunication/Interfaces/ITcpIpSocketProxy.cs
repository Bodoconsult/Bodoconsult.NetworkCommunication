// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Protocols;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for TCP/IP socket implementations
/// </summary>
public interface ITcpIpSocketProxy : ISocketProxy
{
    /// <summary>
    /// Stream pipeline
    /// </summary>
    IStreamPipeline Pipeline { get; }
}