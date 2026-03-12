// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

public static class MessageSendingResultHelper
{
    /// <summary>
    /// Returns an error result
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>Message sending result</returns>
    public static MessageSendingResult Error(string message)
    {
        return new MessageSendingResult(null, OrderExecutionResultState.Error)
        {
            Information = message
        };
    }

    /// <summary>
    /// Returns an error result
    /// </summary>
    /// <returns>Message sending result</returns>
    public static MessageSendingResult Error()
    {
        return new MessageSendingResult(null, OrderExecutionResultState.Error);
    }
}