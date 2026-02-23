// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for request answer definitions
/// </summary>
public interface IRequestAnswer : IDisposable
{
    /// <summary>
    /// Has the answer a datablock?
    /// </summary>
    bool HasDatablock { get; }

    /// <summary>
    /// Type of the data block if <see cref="HasDatablock"/> is true
    /// </summary>
    Type DataBlockType { get; }

    /// <summary>
    /// The answer was successfully received
    /// </summary>
    bool WasReceived { get; }

    /// <summary>
    /// The succesfully received answer from the device
    /// </summary>
    IInboundDataMessage ReceivedMessage { get; }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    HandleRequestAnswerDelegate HandleRequestAnswerOnSuccessDelegate { get; }


    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    HandleUnexpectedRequestAnswerDelegate HandleUnexpectedRequestAnswerDelegate { get; }

    /// <summary>
    /// Check if a received message is the expected answer to the request.
    /// If the message is the requested answer from the device the properties <see cref="WasReceived"/>
    /// and <see cref="ReceivedMessage"/> are set to true and the received message.
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="receivedMessage">A received message from the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors);

    /// <summary>
    /// Set <see cref="WasReceived"/> to true
    /// </summary>
    void SetWasReceived();

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    void Reset();
}