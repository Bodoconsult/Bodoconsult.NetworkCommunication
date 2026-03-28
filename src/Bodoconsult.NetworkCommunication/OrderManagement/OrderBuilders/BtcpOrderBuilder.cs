// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reservBt.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a BTCP order waiting for an answer
/// </summary>
public class BtcpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public BtcpOrderBuilder() : base(typeof(BtcpParameterSet), BuiltinOrders.BtcpOrder)
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
        order.TraceCodeSuccess = TraceCodes.IdsMsgBtcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgBtcpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec");
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;

        var requestAnswerStep = CreateDeviceRequestAnswerStep(requestSpec, "SendAndWaitAnswerStep");

        var requestAnswer = CreateRequestAnswer(requestAnswerStep, "ReceivedMessage", CheckReceivedMessageDelegate, oc.HandleRequestAnswerOnSuccessDelegate);
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

    /// <summary>
    /// Check if a received message is the expected answer to the request.
    /// If the message is the requested answer from the device the properties <see cref="IRequestAnswer.WasReceived"/>
    /// and <see cref="IRequestAnswer.ReceivedMessage"/> are set to true and the received message.
    /// </summary>
    /// <param name="requestAnswer">Current request answer</param>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="receivedMessage">A received message from the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    private static bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors)
    {
        if (receivedMessage is not BtcpRequestInboundDataMessage rm)
        {
            return false;
        }

        if (sentMessage is not BtcpRequestOutboundDataMessage sm)
        {
            return false;
        }

        var erg = sm.BusinessTransactionId == rm.BusinessTransactionId;

        if (erg)
        {
            requestAnswer.SetWasReceived(rm);
        }

        return erg;
    }
}