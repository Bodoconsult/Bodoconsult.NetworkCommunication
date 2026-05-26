// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
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

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    void StartReceiverLoop();

    /// <summary>
    /// Run the receiver loop </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public Task ReceiverLoop(AutoResetEvent waitForLoopStarted);
}