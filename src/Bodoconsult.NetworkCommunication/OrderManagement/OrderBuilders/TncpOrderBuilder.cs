// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a TNCP order waiting for an answer
/// </summary>
public class TncpOrderBuilder : BaseOrderBuilder
{
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory = new TncpOutboundDataMessageFactory();

    /// <summary>
    /// Default ctor
    /// </summary>
    public TncpOrderBuilder() : base(typeof(TncpParameterSet), BuiltinOrders.TncpOrder)
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
        order.TraceCodeSuccess = TraceCodes.IdsMsgTncpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgTncpOrderFails;
        order.TraceMessage = OrderTypeName;

        // RequestSpec 1
        var requestSpec = CreateDeviceRequestSpec(order, "SendAndWaitDeviceRequestSpec");
        requestSpec.CreateMessagesToSentDelegate = CreateMessagesToSentDelegate;

        var requestAnswerStep = CreateDeviceRequestAnswerStep(requestSpec, "SendAndWaitAnswerStep");

        var requestAnswer = CreateRequestAnswer(requestAnswerStep, "ReceivedMessage", CheckReceivedMessageDelegate, oc.HandleRequestAnswerOnSuccessDelegate);
    }

    private List<IOutboundDataMessage> CreateMessagesToSentDelegate(IParameterSet? parameterSet)
    {
        ArgumentNullException.ThrowIfNull(parameterSet);

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
    public static bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors)
    {
        if (receivedMessage is not TncpInboundDataMessage rm)
        {
            return false;
        }

        if (sentMessage is not TncpOutboundDataMessage sm)
        {
            return false;
        }

        if (rm.TelnetCommand == null)
        {
            return false;
        }

        if (!rm.TelnetCommand.StartsWith("<BEGIN>", StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        var send = sm.TelnetCommand?.Replace(" ", string.Empty);

        if (send == null)
        {
            if (sm.DataBlock is TncpParameterSet ps)
            {
                send = ps.TelnetCommand?.Replace(" ", string.Empty);
            }
        }

        if (send == null)
        {
            return false;
        }

        var cmd = rm.TelnetCommand[7..].Replace(" ", string.Empty);
        var result = send == cmd;
        return result;
    }
}