// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for communication handler
/// </summary>
public interface ICommunicationHandler:  IDisposable
{
    /// <summary>
    /// Current <see cref="IDuplexIo"/> instance to use
    /// </summary>
    IDuplexIo DuplexIo { get; }

    /// <summary>
    /// Current data messaging config
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Is the device connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connect to the tower
    /// </summary>
    void Connect();

    /// <summary>
    /// Disconnect from tower
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    MessageSendingResult SendMessage(IOutboundDataMessage message);

    /// <summary>
    /// This method should check if sending a handshake is required and send it if yes. This method is public mainly for testing.
    /// </summary>
    /// <param name="message">Data message received</param>
    void OnReceivedMessage(IInboundDataMessage message);

    /// <summary>
    /// This message should be fired if a message has not been sent
    /// </summary>
    /// <param name="message">Message not sent</param>
    /// <param name="reason">The reason why the message was not sent</param>
    void OnMessageNotSent(ReadOnlyMemory<byte> message, string reason);
}