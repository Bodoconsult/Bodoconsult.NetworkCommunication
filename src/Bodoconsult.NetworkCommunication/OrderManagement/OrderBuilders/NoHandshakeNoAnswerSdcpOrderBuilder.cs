// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a SDCP order waiting for an answer
/// </summary>
public class NoHandshakeNoAnswerSdcpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new SdcpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerSdcpOrderBuilder() : base(typeof(SdcpParameterSet), BuiltinOrders.NoHandshakeNoAnswerSdcpOrder)
    { }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate? HandleRequestAnswerOnSuccessDelegate { get; set; }

    /// <summary>
    /// Configure the order. Implementation of this method may require to add dependencies to your business logic layer
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgSdcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgSdcpOrderFails;
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