// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="INoHandshakeNoAnswerDeviceRequestSpec"/> not expecting a handshake nor an answer
/// </summary>
public class NoHandshakeNoAnswerDeviceRequestSpec : BaseRequestSpec, INoHandshakeNoAnswerDeviceRequestSpec
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="name">Request name</param>
    /// <param name="parameterSet">Current parameter set</param>
    public NoHandshakeNoAnswerDeviceRequestSpec(string name, IParameterSet parameterSet) : base(name, parameterSet)
    {
    }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate HandleRequestAnswerOnSuccessDelegate { get; set; }

    /// <summary>
    /// Current sent message
    /// </summary>
    public IOutboundDataMessage CurrentSentMessage { get; set; }

    /// <summary>
    /// The messages to send. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    public List<IOutboundDataMessage> SentMessage { get; } = new();

    /// <summary>
    /// Send a data message to the device
    /// </summary>
    public SendDataMessageDelegate SendDataMessageDelegate { get; set; }

    /// <summary>
    /// The next step in the chain
    /// </summary>
    public IRequestAnswerStep NextChainElement { get; set; }

    /// <summary>
    /// Delegate for creating data messages to sent to the device
    /// </summary>
    public CreateMessagesToSentDelegate CreateMessagesToSentDelegate { get; set; }

    /// <summary>
    /// Create all messages to process in the step. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    public void CreateMessagesToSend()
    {
        if (CreateMessagesToSentDelegate == null)
        {
            throw new ArgumentNullException(nameof(CreateMessagesToSentDelegate));
        }

        var orders = CreateMessagesToSentDelegate.Invoke(ParameterSet);
        SentMessage.AddRange(orders);
    }
}