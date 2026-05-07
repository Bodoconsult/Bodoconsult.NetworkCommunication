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
    private readonly DuplexIoNoDataDelegate _duplexIoNoDataDelegate;
    private bool _isSending;
    private readonly Lock _isSendingLock = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current device comm settings</param>
    /// <param name="duplexIoIsWorkInProgressDelegate">Delegate for checking if the socket is wokring currently</param>
    /// <param name="duplexIoNoDataDelegate">Delegate to set socket state to no work in progress</param>
    public UdpDatagramIpDuplexIoSender(IDataMessagingConfig dataMessagingConfig,
        DuplexIoIsWorkInProgressDelegate duplexIoIsWorkInProgressDelegate,
        DuplexIoNoDataDelegate duplexIoNoDataDelegate) : base(dataMessagingConfig)
    {
        _duplexIoNoDataDelegate = duplexIoNoDataDelegate;
    }

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
                msg = $"Encoding for message failed: {message.MessageId}";
                DataMessagingConfig.MonitorLogger.LogInformation(msg);
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                return 0;
            }

            // Send message
            var sent = await SendMessageInternal(DataMessagingConfig, _duplexIoNoDataDelegate, message);

            if (sent > 0)
            {
                return sent;
            }

            msg = "message could not be sent via UDP socket";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
        }
        catch (Exception e)
        {
            msg = $"message could not be sent via UDP socket: {e}";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
        }

        return 0;
    }

    private async Task<int> SendMessageInternal(IDataMessagingConfig dataMessagingConfig, DuplexIoNoDataDelegate duplexIoNoDataDelegate, IOutboundMessage message)
    {
        string msg;
        int sent;
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
            duplexIoNoDataDelegate();

            lock (_isSendingLock)
            {
                _isSending = false;
            }

            if (sent == 0)
            {
                return sent;
            }

#if DEBUG
            msg = $"{message.RawMessageDataClearText}  {message.ToShortInfoString()}";
            dataMessagingConfig.MonitorLogger.LogDebug($"Message sent {msg}: {message.RawMessageData.Length}");
#endif
            AsyncHelper.FireAndForget(() =>
            {
                dataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData);
            });
        }
        catch (SocketException socketException)
        {
            lock (_isSendingLock)
            {
                _isSending = false;
            }

            msg = $"message {message.ToShortInfoString()} not sent: {socketException}";
            dataMessagingConfig.MonitorLogger.LogInformation(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                dataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("UdpDatagramIpDuplexIoSender");
                dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message);
            });
            throw;
        }
        catch (Exception sendException)
        {
            lock (_isSendingLock)
            {
                _isSending = false;
            }

            msg = $"message {message.ToShortInfoString()} not sent: {sendException}";
            dataMessagingConfig.MonitorLogger.LogInformation(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() => dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, sendException.Message));
            throw;
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