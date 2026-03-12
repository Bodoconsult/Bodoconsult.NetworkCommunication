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
    public MessageSendingResult(IOutboundMessage? message, IOrderExecutionResultState processExecutionResult)
    {
        Message = message;
        ProcessExecutionResult = processExecutionResult;
    }

    /// <summary>
    /// Message sent
    /// </summary>
    public IOutboundMessage? Message { get; }

    /// <summary>
    /// Process execution result
    /// </summary>
    public IOrderExecutionResultState ProcessExecutionResult { get; }

    /// <summary>
    /// Additonal information
    /// </summary>
    public string? Information { get; set; }
}