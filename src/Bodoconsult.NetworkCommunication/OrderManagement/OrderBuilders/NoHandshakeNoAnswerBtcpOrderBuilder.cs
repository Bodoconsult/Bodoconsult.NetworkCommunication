// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a BTCP order not waiting for an answer
/// </summary>
public class NoHandshakeNoAnswerBtcpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerBtcpOrderBuilder() : base(typeof(BtcpParameterSet), BuiltinOrders.NoHandshakeNoAnswerBtcpOrder)
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
        order.TraceCodeSuccess = TraceCodes.IdsMsgBtcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgBtcpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateNoHandshakeNoAnswerDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec", HandleRequestAnswerOnSuccessDelegate);
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;
    }

    private List<IOutboundDataMessage> CreateMessagesToSentDelegate(IParameterSet? parameterSet)
    {
        if (parameterSet == null)
        {
            throw new ArgumentNullException(nameof(parameterSet));
        }

        var msg = _outboundDataMessageFactory.CreateInstance(parameterSet);
        msg.WaitForAcknowledgement = false;
        return [msg];
    }
}