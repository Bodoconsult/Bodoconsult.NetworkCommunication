// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a EDCP client order ONLY waiting for an answer
/// </summary>
public class OnlyAnswerEdcpServerOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new EdcpServerOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public OnlyAnswerEdcpServerOrderBuilder() : base(typeof(EdcpParameterSet), BuiltinOrders.OnlyAnswerEdcpServerOrder)
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
            throw new ArgumentException($"Config must be {nameof(OneRequestSpecNoOrOneStepOneAnswerConfiguration)} but was {config.GetType().Name}");
        }

        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgEdcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgEdcpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateOnlyAnswerDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec", oc.HandleRequestAnswerOnSuccessDelegate);
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;


        var requestAnswerStep = CreateDeviceRequestAnswerStep(requestSpec, "SendAndWaitAnswerStep");

        var requestAnswer = CreateRequestAnswer(requestAnswerStep, "ReceivedMessage", EdcpServerOrderBuilder.CheckReceivedMessageDelegate, oc.HandleRequestAnswerOnSuccessDelegate);

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