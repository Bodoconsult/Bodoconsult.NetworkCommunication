// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for the receiver side of an <see cref="IDuplexIo"/> based communication interface
/// </summary>
public interface IDuplexIoReceiver: IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Start the internal receiver
    /// </summary>
    Task StartReceiver();

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    Task StopReceiver();

    /// <summary>
    /// Receive messages from the device.
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    /// <returns>Received device message or null in case of any error</returns>
    Task FillMessagePipeline();

    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    Task SendMessagePipeline();

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    void UpdateDataMessageProcessingPackage();
}