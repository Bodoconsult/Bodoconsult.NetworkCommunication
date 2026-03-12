// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a TNCP order not waiting for an answer
/// </summary>
public class NoAnswerTncpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new TncpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoAnswerTncpOrderBuilder() : base(typeof(TncpParameterSet), BuiltinOrders.NoAnswerTncpOrder)
    { }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate? HandleRequestAnswerOnSuccessDelegate { get; set; }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgTncpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgTncpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateNoAnswerDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec", HandleRequestAnswerOnSuccessDelegate);
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;

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