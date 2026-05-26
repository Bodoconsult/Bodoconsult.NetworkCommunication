// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net.Sockets;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message sending version 1 for IP based networks for UDP only
/// </summary>
public class UdpDatagramIpDuplexIoSender : BaseDuplexIoSender
{
    private bool _isSending;
    private readonly Lock _isSendingLock = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current device comm settings</param>
    public UdpDatagramIpDuplexIoSender(IDataMessagingConfig dataMessagingConfig) : base(dataMessagingConfig)
    { }

    /// <summary>
    /// Start the message sender
    /// </summary>
    public override async Task StartSender()
    {
        // Do nothing
        await Task.Run(() => { });
    }

    /// <summary>
    /// Stop the message sender
    /// </summary>
    public override async Task StopSender()
    {
        // Do nothing
        await Task.Run(() => { });
    }


    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public override async Task<int> SendMessage(IOutboundMessage message)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.SocketProxy);

        string msg;
        try
        {
            //Trace.TraceInformation("Send really");

            if (EncodeMessage(message))
            {
                msg = $"Encoding for message {message.ToShortInfoString()} failed: {message.MessageId}";
                DataMessagingConfig.MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                return 0;
            }

            // Send message
            var sent = await SendMessageInternal(DataMessagingConfig,  message);

            if (sent > 0)
            {
                return sent;
            }

            msg = $"Message {message.ToShortInfoString()} could not be sent via UDP socket";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}: {msg}");
        }
        catch (Exception e)
        {
            msg = $"Message {message.ToShortInfoString()} could not be sent via UDP socket: {e}";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
        }

        return 0;
    }

    private async Task<int> SendMessageInternal(IDataMessagingConfig dataMessagingConfig, IOutboundMessage message)
    {
        string msg;
        var sent = 0;
        try
        {
            lock (_isSendingLock)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (_isSending)
                    {
                        Task.Delay(15);
                    }
                    else
                    {
                        break;
                    }
                }

                if (_isSending)
                {
                    return 0;
                }

                _isSending = true;
            }

            sent = await dataMessagingConfig.SocketProxy!.Send(message.RawMessageData);

            lock (_isSendingLock)
            {
                _isSending = false;
            }

            if (sent == 0)
            {
                return 0;
            }

#if DEBUG
            msg = $"Message {message.ToShortInfoString()} sent: {message.RawMessageData.Length} bytes";
            dataMessagingConfig.MonitorLogger.LogDebug(msg);
#endif
            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    dataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData);
                }
                catch (Exception e)
                {
                    msg = $"dataMessagingConfig.RaiseDataMessageSentDelegate failed: {e}";
                    dataMessagingConfig.MonitorLogger.LogError(msg);
                    dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
                }
            });
        }
        catch (SocketException socketException)
        {
            lock (_isSendingLock)
            {
                _isSending = false;
            }

            msg = $"message {message.ToShortInfoString()} not sent: {socketException}";
            dataMessagingConfig.MonitorLogger.LogError(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    dataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("UdpDatagramIpDuplexIoSender");
                    dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message);
                }
                catch (Exception e)
                {
                    msg = $"firing delegates failed: {e}";
                    dataMessagingConfig.MonitorLogger.LogError(msg);
                    dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
                }

            });
        }
        catch (Exception sendException)
        {
            lock (_isSendingLock)
            {
                _isSending = false;
            }

            msg = $"message {message.ToShortInfoString()} not sent: {sendException}";
            dataMessagingConfig.MonitorLogger.LogError(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                try
                {
                    dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, sendException.Message);
                }
                catch (Exception e)
                {
                    msg = $"dataMessagingConfig.RaiseDataMessageNotSentDelegate failed: {e}";
                    dataMessagingConfig.MonitorLogger.LogError(msg);
                    dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
                }
            });
        }

        return sent;
    }

    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected override async Task Dispose(bool disposing)
    {
        if (!disposing)
        {
        }

        await Task.Run(() => { });
    }
}