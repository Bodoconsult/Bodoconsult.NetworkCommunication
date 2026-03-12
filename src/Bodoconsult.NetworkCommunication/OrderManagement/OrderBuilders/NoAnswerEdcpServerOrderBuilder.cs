// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a EDCP server order not waiting for an answer
/// </summary>
public class NoAnswerEdcpServerOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new EdcpServerOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoAnswerEdcpServerOrderBuilder() : base(typeof(EdcpParameterSet), BuiltinOrders.NoAnswerEdcpServerOrder)
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
        order.TraceCodeSuccess = TraceCodes.IdsMsgEdcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgEdcpOrderFails;
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