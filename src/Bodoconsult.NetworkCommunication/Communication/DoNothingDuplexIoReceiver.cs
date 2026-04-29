// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// An <see cref="IDuplexIoReceiver"/> doing nothing
/// </summary>
public class DoNothingDuplexIoReceiver : IDuplexIoReceiver
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
    /// Thread filling receiver pipeline
    /// </summary>
    public Thread? FillPipelineTask { get; set; }

    /// <summary>
    /// Thread sending messages from receiver pipeline to app internal consumers
    /// </summary>
    public Thread? SendPipelineTask { get; set; }

    /// <summary>
    /// Start the internal receiver
    /// </summary>
    public Task StartReceiver()
    {
        // Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public Task StopReceiver()
    {
        // Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    /// Receive messages from the device.
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    /// <returns>Received device message or null in case of any error</returns>
    public Task FillMessagePipeline()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    public Task SendMessagePipeline()
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