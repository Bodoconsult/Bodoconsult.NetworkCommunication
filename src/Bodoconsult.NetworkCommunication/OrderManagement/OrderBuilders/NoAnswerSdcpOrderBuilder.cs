// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a SDCP order not waiting for an answer
/// </summary>
public class NoAnswerSdcpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new SdcpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoAnswerSdcpOrderBuilder() : base(typeof(SdcpParameterSet), BuiltinOrders.NoAnswerSdcpOrder)
    { }

    /// <summary>
    /// Configure the order. Implementation of this method may require to add dependencies to your business logic layer
    /// </summary>
    /// <param name="order">Current order to configure</param>
    /// <param name="config">Current configuration</param>
    public override void ConfigureOrder(IOrder order, IOrderConfiguration config)
    {
        if (config is not OneRequestSpecNoOrOneStepOneAnswerConfiguration oc)
        {
            throw new ArgumentException(
                $"Config must be {nameof(OneRequestSpecNoOrOneStepOneAnswerConfiguration)} but was {{config.GetType().Name}}");
        }

        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgSdcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgSdcpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateNoAnswerDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec", oc.HandleRequestAnswerOnSuccessDelegate);
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