// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for UDP implementations of <see cref="ISocketProxy"/>
/// </summary>
public interface ITcpSocketProxy : ISocketProxy
{
    /// <summary>
    /// Start the receiver loop
    /// </summary>
    void StartReceiverLoop();
}