// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoSender"/> implementations
/// </summary>
public abstract class BaseDuplexIoSender : IDuplexIoSender
{
    /// <summary>
    /// Current device comm settings
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get;  }

    /// <summary>
    /// Current data messaging coding processor impl
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; private set; }

    /// <summary>
    /// Current data message splitter impl
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; private set; }

    public BaseDuplexIoSender(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig =dataMessagingConfig;
        UpdateDataMessageProcessingPackage();
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual Task<int> SendMessage(IDataMessage message)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Start the message sender
    /// </summary>
    public virtual Task StartSender()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Stop the message sender
    /// </summary>
    public virtual Task StopSender()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageSplitter = DataMessagingConfig.DataMessageProcessingPackage.DataMessageSplitter;
    }


    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected virtual async Task Dispose(bool disposing)
    {
        if (!disposing)
        {
        }

        await Task.Run(() => { });
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public async ValueTask DisposeAsync()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true).Wait(1000);
        GC.SuppressFinalize(this);
    }
}