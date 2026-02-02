// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoReceiver"/> implementations
/// </summary>
public class BaseDuplexIoReceiver : IDuplexIoReceiver
{
    protected int PollingTimeOut;

    //private readonly int _timeOut = 2000;



    /// <summary>
    /// Current device comm settings
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data message splitter
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; private set; }

    /// <summary>
    /// Current data message coding processor
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; private set; }

    /// <summary>
    /// Current data message processor for internal forwarding of the received messages
    /// </summary>
    public IDataMessageProcessor DataMessageProcessor { get; private set; }


    public BaseDuplexIoReceiver(IDataMessagingConfig deviceCommSettings)
    {
        DataMessagingConfig = deviceCommSettings;
        UpdateDataMessageProcessingPackage();
    }

        

    /// <summary>
    /// Start the internal receiver
    /// </summary>
    public virtual Task StartReceiver()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public virtual Task StopReceiver()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Receive messages from the device.
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    /// <returns>Received device message or null in case of any error</returns>
    public virtual Task FillMessagePipeline()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    public virtual Task SendMessagePipeline()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        //if (DataMessagingConfig.DataMessageProcessingPackage==null)
        //{
        //    return;
        //}
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageProcessor;
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