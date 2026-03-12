// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Fake implementation of <see cref="IOrderReceiver"/>
/// </summary>
public class FakeOrderReceiver : IOrderReceiver
{
    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing
    }

    /// <summary>
    /// Delegate for handling a received device message
    /// </summary>
    public OrderReceiverCheckMessageDelegate? OrderReceiverCheckMessageDelegate { get; set; }

    /// <summary>
    /// Is the received message processing activated?
    /// </summary>
    public bool IsReceivedMessageProcessingActivated { get; set; }

    /// <summary>
    /// Adds a received message to the receiver queue for further processing
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public void AddReceivedMessage(IInboundDataMessage receivedMessage)
    {
        // Do nothing
    }
}