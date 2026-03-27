// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.OrderBuilders;

internal class OrderBuilderTestsBase
{
    protected MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        // Do nothing
        return new MessageHandlingResult
        {
            Error = 0,
            ExecutionResult = OrderExecutionResultState.Successful
        };
    }

    protected bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage? sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors)
    {
        if (receivedMessage == null)
        {
            return false;
        }
        requestAnswer.SetWasReceived(receivedMessage);
        return true;
    }
}