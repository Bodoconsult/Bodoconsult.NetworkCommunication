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
    /// Configure the order. Implementation of this method may require to add dependencies to your business logic layer
    /// </summary>
    /// <param name="order">Current order to configure</param>
    /// <param name="config">Current configuration</param>
    public override void ConfigureOrder(IOrder order, IOrderConfiguration config)
    {
        //if (config is not " + nameof(OneRequestSpecNoOrOneStepOneAnswerConfiguration) + " oc)
        //{
        //    throw new ArgumentException($"Config must be " + nameof(OneRequestSpecNoOrOneStepOneAnswerConfiguration) + " but was {config.GetType().Name}");
        //}

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

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        Task.Delay(300);
        return new MessageHandlingResult
        {
            ExecutionResult = OrderExecutionResultState.Successful
        };
    }

    private List<IOutboundDataMessage> CreateMessagesToSentDelegate(IParameterSet? parameterSet)
    {
        if (parameterSet == null)
        {
            throw new ArgumentNullException(nameof(parameterSet));
        }

        var msg = _outboundDataMessageFactory.CreateInstance(parameterSet);
        msg.WaitForAcknowledgement = true;
        return [msg];
    }
}