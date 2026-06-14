// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.SyncExecution;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoSender"/> implementations
/// </summary>
public abstract class BaseDuplexIoSender : IDuplexIoSender
{
    private readonly SyncProcessManager<long, MessageSendingResult> _outboundProcessManager = new();
    private readonly ProducerConsumerQueue<IOutboundMessage> _outboundConsumerQueue = new();

    /// <summary>
    /// Logger ID
    /// </summary>
    protected string LoggerId;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected async Task<MessageSendingResult> SendMessageInternal(IOutboundMessage message)
    {
        string msg;
        MessageSendingResult sent;
        try
        {
            var syncData = _outboundProcessManager.AddSyncProcess(message.MessageId, 2000);
            message.SyncData = syncData;

            _outboundConsumerQueue.Enqueue(message);

            sent = await syncData.CreateWaitingTask() ?? new MessageSendingResult(message, OrderExecutionResultState.Timeout);

            if (sent.ProcessExecutionResult == OrderExecutionResultState.Unsuccessful)
            {
                return sent;
            }

#if DEBUG
            DataMessagingConfig.MonitorLogger.LogDebug($"{LoggerId}{message.ToShortInfoString()} sent {sent.BytesSent}B: {sent.ProcessExecutionResult}");
#endif

//#if DEBUG
//            msg = $"Message {message.ToShortInfoString()} sent: {sent.BytesSent}B";
//            DataMessagingConfig.MonitorLogger.LogDebug(msg);
//#endif
            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    DataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData);
                }
                catch (Exception e)
                {
                    msg = $"dataMessagingConfig.RaiseDataMessageSentDelegate failed: {e}";
                    DataMessagingConfig.MonitorLogger.LogError(msg);
                    DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                }
            });

            return sent;
        }
        catch (Exception sendException)
        {
            msg = $"{message.ToShortInfoString()} not sent: {sendException}";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, sendException.Message);
                }
                catch (Exception e)
                {
                    msg = $"dataMessagingConfig.RaiseDataMessageNotSentDelegate failed: {e}";
                    DataMessagingConfig.MonitorLogger.LogError(msg);
                    DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                }
            });

            sent = new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }

        return sent;
    }

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
                DataMessagingConfig.MonitorLogger.LogError($"Encoding for message failed: {message.MessageId}: {s}");
                DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, s);
                DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new Exception(s));
                return true;
            }
        }
        catch (Exception encodeException)
        {
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

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    protected BaseDuplexIoSender(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;
        LoggerId = $"{dataMessagingConfig.LoggerId}{(dataMessagingConfig.LoggerId.EndsWith(": ") ? string.Empty : ": ")}{GetType().Name}: ";
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageSplitter = DataMessagingConfig.DataMessageProcessingPackage.DataMessageSplitter;
    }

    private void ConsumerTaskDelegate(IOutboundMessage message)
    {
        try
        {
            var sent = DataMessagingConfig.SocketProxy!.Send(message.RawMessageData).GetAwaiter().GetResult();

            if (message.SyncData == null)
            {
                return;
            }

            var msr = sent > 0
                ? new MessageSendingResult(message, OrderExecutionResultState.Successful)
                : new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
            msr.BytesSent = sent;

            message.SyncData.TaskCompletionSource.SetResult(msr);
        }
        catch (SocketException socketException)
        {
            var msg = $"message {message.ToShortInfoString()} not sent: {socketException}";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("IpDuplexIoSender");
                    DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message);
                }
                catch (Exception e)
                {
                    msg = $"firing delegates failed: {e}";
                    DataMessagingConfig.MonitorLogger.LogError(msg);
                    DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                }
            });

            if (message.SyncData == null)
            {
                return;
            }

            message.SyncData.SetResult(new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful));
        }
        catch (Exception e)
        {
            var msg = $"Message {message.ToShortInfoString()} could not be sent: {e}";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}: {msg}");

            if (message.SyncData == null)
            {
                return;
            }
            message.SyncData.SetResult(new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful));
        }
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual Task<MessageSendingResult> SendMessage(IOutboundMessage message)
    {
        throw new NotSupportedException();
    }



    /// <summary>
    /// Start the message sender
    /// </summary>
    public virtual async Task StartSender()
    {
        // Do nothing
        await Task.Run(() =>
        {
            _outboundConsumerQueue.ConsumerTaskDelegate = ConsumerTaskDelegate;
            _outboundConsumerQueue.StartConsumer();
        });
    }

    /// <summary>
    /// Stop the message sender
    /// </summary>
    public async Task StopSender()
    {
        // Do nothing
        await Task.Run(() =>
        {
            _outboundConsumerQueue.StopConsumer();
        });
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