// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a test order waiting for an answer. Accepts any datablock type
/// </summary>
public class TestOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new SdcpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public TestOrderBuilder() : base(null, BuiltinOrders.TestOrder)
    { }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgTestOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgTestOrderFails;
        order.TraceMessage = OrderTypeName;

        // Parallel orders
        order.AllowedParallelOrderTypes.Add(BuiltinOrders.LongRunningTestOrder);
        order.AllowedParallelOrderTypes.Add(BuiltinOrders.ExtraLongRunningTestOrder);

        // RequestSpec 1
        var requestSpec = CreateNoAnswerDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec", HandleRequestAnswerOnSuccessDelegate);
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;
    }

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage message, object transportObject, IParameterSet parameterSet)
    {
        Task.Delay(300);
        return new MessageHandlingResult
        {
            ExecutionResult = OrderExecutionResultState.Successful
        };
    }

    private List<IOutboundDataMessage> CreateMessagesToSentDelegate(IParameterSet parameterSet)
    {
        var msg = _outboundDataMessageFactory.CreateInstance(parameterSet);
        msg.WaitForAcknowledgement = true;
        return [msg];
    }
}