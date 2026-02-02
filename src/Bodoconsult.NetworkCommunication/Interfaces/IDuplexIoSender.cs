// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for the sender side of an <see cref="IDuplexIo"/> based communication interface
/// </summary>
public interface IDuplexIoSender: IAsyncDisposable, IDisposable

{
    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    Task<int> SendMessage(IDataMessage message);

    /// <summary>
    /// Start the message sender
    /// </summary>
    Task StartSender();

    /// <summary>
    /// Stop the message sender
    /// </summary>
    Task StopSender();

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    void UpdateDataMessageProcessingPackage();

}