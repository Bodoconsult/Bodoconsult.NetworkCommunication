// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Message sending result
/// </summary>
public class MessageSendingResult
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="message">Message sent</param>
    /// <param name="processExecutionResult">Process execution result</param>
    public MessageSendingResult(IOutboundDataMessage message, IOrderExecutionResultState processExecutionResult)
    {
        Message = message;
        ProcessExecutionResult = processExecutionResult;
    }

    /// <summary>
    /// Message sent
    /// </summary>
    public IOutboundDataMessage Message { get; }

    /// <summary>
    /// Process execution result
    /// </summary>
    public IOrderExecutionResultState ProcessExecutionResult { get; }
}