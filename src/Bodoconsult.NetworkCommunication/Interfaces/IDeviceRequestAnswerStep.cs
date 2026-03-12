// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for defining a device request answer step
/// </summary>
public interface IDeviceRequestAnswerStep : IRequestAnswerStep
{
    /// <summary>
    /// Device bound request spec
    /// </summary>
    IDeviceRequestSpec DeviceRequestSpec { get; }

    /// <summary>
    /// The accepted message leading <see cref="IRequestAnswer.WasReceived"/> being true
    /// </summary>
    IInboundDataMessage? AcceptedMessage { get; }

    /// <summary>
    /// Next chain element
    /// </summary>
    IDeviceRequestAnswerStep? NextChainElement { get; set; }

    /// <summary>
    /// Check the received message is fitting to the current request
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the received message is fitting to the current request else false</returns>
    IEnumerable<string> CheckReceivedMessage(IInboundDataMessage? receivedMessage);

    /// <summary>
    /// Check if a received message is the expected answer to the request step.
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="receivedMessage">A received message from the device</param>
    /// <param name="errors">List with error messages to fill</param> <returns>True if the message was as expected as answer of the sent message else false</returns>
    bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors);

    /// <summary>
    /// Process the current request answer step in a chain
    /// </summary>
    public void ProcessChainElement();
}