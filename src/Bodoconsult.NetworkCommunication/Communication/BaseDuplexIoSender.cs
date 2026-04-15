// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoSender"/> implementations
/// </summary>
public abstract class BaseDuplexIoSender : IDuplexIoSender
{
    /// <summary>
    /// Encode the data message
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <returns>True if the message was NOT encodeable else false</returns>
    protected bool EncodeMessage(IOutboundMessage message)
    {
        // Encode message
        try
        {
            var result = DataMessageCodingProcessor.EncodeDataMessage(message);

            if (result.ErrorCode != 0)
            {
                var s = result.ErrorMessage ?? "Unknown";
                Trace.TraceError($"Encoding for message failed: {message.MessageId}: {s}");
                DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, s);
                DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new Exception(s));
                return true;
            }
        }
        catch (Exception encodeException)
        {
            Trace.TraceError($"Encoding for message failed: {message.MessageId}: {encodeException}");
            DataMessagingConfig.MonitorLogger.LogError("Encoding message to send failed", encodeException);
            DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(null, encodeException.Message);
            DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(encodeException);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Current device comm settings
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data messaging coding processor impl
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; private set; }

    /// <summary>
    /// Current data message splitter impl
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; private set; }

    protected BaseDuplexIoSender(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageSplitter = DataMessagingConfig.DataMessageProcessingPackage.DataMessageSplitter;
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual Task<int> SendMessage(IOutboundMessage message)
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
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);
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