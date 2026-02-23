// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement;

/// <summary>
/// Base class for request answers
/// </summary>
public abstract class BaseRequestAnswer : IRequestAnswer
{
    /// <summary>
    /// Has the answer a datablock?
    /// </summary>
    public bool HasDatablock { get; protected set; }

    /// <summary>
    /// Type of the data block if <see cref="IRequestAnswer.HasDatablock"/> is true
    /// </summary>
    public Type DataBlockType { get; protected set; }

    /// <summary>
    /// The answer was successfully received
    /// </summary>
    public bool WasReceived { get; protected set; }

    /// <summary>
    /// The succesfully received answer from the device
    /// </summary>
    public IInboundDataMessage ReceivedMessage { get; protected set; }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate HandleRequestAnswerOnSuccessDelegate { get; protected set; }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleUnexpectedRequestAnswerDelegate HandleUnexpectedRequestAnswerDelegate { get; protected set; }

    /// <summary>
    /// Check if a received message is the expected answer to the request.
    /// If the message is the requested answer from the device the properties <see cref="IRequestAnswer.WasReceived"/>
    /// and <see cref="IRequestAnswer.ReceivedMessage"/> are set to true and the received message.
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="receivedMessage">A received message from the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    public virtual bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Set <see cref="IRequestAnswer.WasReceived"/> to true
    /// </summary>
    public void SetWasReceived()
    {
        WasReceived = true;
    }

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    public void Reset()
    {
        WasReceived = false;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        ReceivedMessage = null;
        HandleRequestAnswerOnSuccessDelegate = null;
        HandleUnexpectedRequestAnswerDelegate = null;
    }
}