// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for duplex (bi-directional) comm channels
/// </summary>
public interface IDuplexIo: IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Is the communication started?
    /// </summary>
    bool IsCommunicationStarted { get; }

    /// <summary>
    /// The receiver part used of the duplex (bi-directional) comm channels
    /// </summary>
    IDuplexIoReceiver Receiver { get;  }

    /// <summary>
    /// The sender part used of the duplex (bi-directional) comm channels
    /// </summary>
    IDuplexIoSender Sender { get; }

       
    /// <summary>
    /// Current data messaging config
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }


    /// <summary>
    /// Is the current connection alive? True, if yes else false.
    /// </summary>
    bool IsConnectionAlive { get; }

    /// <summary>
    /// Send a message to the device.
    /// </summary>
    /// <param name="message">Current message to send</param>
    Task<MessageSendingResult> SendMessage(IDataMessage message);

    /// <summary>
    /// Send a message to the device directly. This method is intended for internal purposes only. Do NOT use directly. Use <see cref="SendMessage"/> instead. This method makes faking easier!
    /// </summary>
    /// <param name="message">Current message to send</param>
    Task<MessageSendingResult> SendMessageInternal(IDataMessage message);

    ///// <summary>
    ///// Starts the message sending process either with waiting for a response from devive or without it
    ///// </summary>
    ///// <param name="message">Current message to send</param>
    ///// <returns>Result of the message sending process</returns>
    //MessageSendingResult StartMessageSendingProcess(IDataMessage message);

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    Task StartCommunication();

    /// <summary>
    /// Stop the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    Task StopCommunication();

    ///// <summary>
    ///// Get the answer of a device firmware request for an old devices (firmware older than 100)
    ///// </summary>
    ///// <returns>Rceived firmware byte array</returns>
    //Task<byte[]> ReceiveFwUpdateMessageFromdevice();

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    void UpdateDataMessageProcessingPackage();

}