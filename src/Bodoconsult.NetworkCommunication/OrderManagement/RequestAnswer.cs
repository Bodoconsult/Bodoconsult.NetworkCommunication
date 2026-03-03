// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement;

/// <summary>
/// Request answers
/// </summary>
public class RequestAnswer : IRequestAnswer
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="hasDatablock">Has the answer a datablock?</param>
    /// <param name="dataBlockType">Type of the data block if <see cref="IRequestAnswer.HasDatablock"/> is true</param>
    /// <param name="name">A cleartext name of the request answer for logging</param>
    public RequestAnswer(bool hasDatablock, Type dataBlockType, string name)
    {
        HasDatablock = hasDatablock;
        DataBlockType = dataBlockType;
        Name = name;
    }

    /// <summary>
    /// Name of the request answer
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Has the answer a datablock?
    /// </summary>
    public bool HasDatablock { get; }

    /// <summary>
    /// Type of the data block if <see cref="IRequestAnswer.HasDatablock"/> is true
    /// </summary>
    public Type DataBlockType { get; }

    /// <summary>
    /// The answer was successfully received
    /// </summary>
    public bool WasReceived { get; private set; }

    /// <summary>
    /// The succesfully received answer from the device
    /// </summary>
    public IInboundDataMessage ReceivedMessage { get; private set; }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate HandleRequestAnswerOnSuccessDelegate { get; set; }

    ///// <summary>
    ///// Delegate for handling request answer messages
    ///// </summary>
    //public HandleUnexpectedRequestAnswerDelegate HandleUnexpectedRequestAnswerDelegate { get; protected set; }

    /// <summary>
    /// Check if a received message is the expected answer to the request.
    /// If the message is the requested answer from the device the properties <see cref="IRequestAnswer.WasReceived"/>
    /// and <see cref="IRequestAnswer.ReceivedMessage"/> are set to true and the received message.
    /// </summary>
    public CheckReceivedMessageDelegate CheckReceivedMessageDelegate { get; set; }

    /// <summary>
    /// Set <see cref="IRequestAnswer.WasReceived"/> to true
    /// </summary>
    /// <param name="receivedMessage">Received message</param>>
    public void SetWasReceived(IInboundDataMessage receivedMessage)
    {
        WasReceived = true;
        ReceivedMessage = receivedMessage;
    }

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    public void Reset()
    {
        WasReceived = false;
        ReceivedMessage = null;
        HandleRequestAnswerOnSuccessDelegate = null;
        //HandleUnexpectedRequestAnswerDelegate = null;
        CheckReceivedMessageDelegate = null;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Reset();
    }
}