// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for request answer definitions
/// </summary>
public interface IRequestAnswer : IDisposable
{
    /// <summary>
    /// Name of the request answer
    /// </summary>
    string Name { get; }

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

    ///// <summary>
    ///// Delegate for handling request answer messages
    ///// </summary>
    //HandleUnexpectedRequestAnswerDelegate HandleUnexpectedRequestAnswerDelegate { get; }

    /// <summary>
    /// Check if a received message is the expected answer to the request.
    /// If the message is the requested answer from the device the properties <see cref="IRequestAnswer.WasReceived"/>
    /// and <see cref="IRequestAnswer.ReceivedMessage"/> are set to true and the received message.
    /// </summary>
    CheckReceivedMessageDelegate CheckReceivedMessageDelegate { get; set; }

    /// <summary>
    /// Set <see cref="WasReceived"/> to true
    /// </summary>
    /// <param name="receivedMessage">Received message</param>>
    void SetWasReceived(IInboundDataMessage receivedMessage);

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    void Reset();
}