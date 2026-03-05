// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for device request specifications waiting for no answer(s)
/// </summary>
public interface INoAnswerDeviceRequestSpec : IRequestSpec
{
    /// <summary>
    /// The expected handshake if the message was sent
    /// </summary>
    List<IOrderExecutionResultState> ExpectedHandshakeForSentMessage { get; }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    HandleRequestAnswerDelegate HandleRequestAnswerOnSuccessDelegate { get; set; }

    /// <summary>
    /// Current sent message
    /// </summary>
    IOutboundDataMessage CurrentSentMessage { get; set; }

    /// <summary>
    /// The messages to send. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    List<IOutboundDataMessage> SentMessage { get; }

    /// <summary>
    /// Send a data message to the device
    /// </summary>
    SendDataMessageDelegate SendDataMessageDelegate { get; set; }

    /// <summary>
    /// The next step in the chain
    /// </summary>
    IRequestAnswerStep NextChainElement { get; set; }

    /// <summary>
    /// Delegate for creating data messages to sent to the device
    /// </summary>
    CreateMessagesToSentDelegate CreateMessagesToSentDelegate { get; set; }

    /// <summary>
    /// Create all messages to process in the step. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    void CreateMessagesToSend();
}