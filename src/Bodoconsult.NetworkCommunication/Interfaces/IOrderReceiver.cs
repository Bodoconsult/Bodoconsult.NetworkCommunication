// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// The receiver module of the tower order management
/// </summary>
public interface IOrderReceiver: IDisposable
{

    /// <summary>
    /// Delegate for handling a received tower message
    /// </summary>
    OrderReceiverCheckMessageDelegate OrderReceiverCheckMessageDelegate { get; set; }

    /// <summary>
    /// Is the received message processing activated?
    /// </summary>
    bool IsReceivedMessageProcessingActivated { get; set; }


    /// <summary>
    /// Adds a received message to the receiver queue for further processing
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    void AddReceivedMessage(IDataMessage receivedMessage);

}