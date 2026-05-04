// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// An <see cref="IDuplexIoSender"/> doing nothing
/// </summary>
public class DoNothingDuplexIoSender : IDuplexIoSender
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing 
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public Task<int> SendMessage(IOutboundMessage message)
    {
        return Task.FromResult(0);
    }

    /// <summary>
    /// Start the message sender
    /// </summary>
    public Task StartSender()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop the message sender
    /// </summary>
    public Task StopSender()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        // Do nothing
    }
}