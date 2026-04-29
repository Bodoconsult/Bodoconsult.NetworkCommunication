// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net.Sockets;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Microsoft.VisualBasic;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message sending version 1 for IP based networks (UDP/TCP)
/// </summary>
public class IpDuplexIoSender : BaseDuplexIoSender
{

    private readonly DuplexIoIsWorkInProgressDelegate _duplexIoIsWorkInProgressDelegate;
    private readonly DuplexIoNoDataDelegate _duplexIoNoDataDelegate;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current device comm settings</param>
    /// <param name="duplexIoIsWorkInProgressDelegate">Delegate for checking if the socket is wokring currently</param>
    /// <param name="duplexIoNoDataDelegate">Delegate to set socket state to no work in progress</param>
    public IpDuplexIoSender(IDataMessagingConfig dataMessagingConfig,
        DuplexIoIsWorkInProgressDelegate duplexIoIsWorkInProgressDelegate,
        DuplexIoNoDataDelegate duplexIoNoDataDelegate) : base(dataMessagingConfig)
    {
        _duplexIoIsWorkInProgressDelegate = duplexIoIsWorkInProgressDelegate;
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

        var sent = 0;

        var i = 0;
        var isSent = false;

        if (EncodeMessage(message))
        {
            return 0;
        }

        // Send message
        try
        {
            //Create a byte array from message according to current protocol
            while (i < 3)
            {
                if (!_duplexIoIsWorkInProgressDelegate())
                {
                    (sent, isSent) = await SendMessageInternal(DataMessagingConfig, _duplexIoNoDataDelegate, message);

                    Trace.TraceInformation($"{LoggerId}Sent: {sent} // {isSent}");

                    if (isSent)
                    {
                        break;
                    }
                }

                AsyncHelper.Delay(10);
                i++;
            }

            if (!isSent)
            {
                DataMessagingConfig.MonitorLogger.LogError("send process blocked by another socket operation");
                return 0;
            }

            if (sent > 0)
            {
                return sent;
            }
            var msg = $"message could not be sent via TCP socket. Only {0} bytes of {message.RawMessageData.Length} bytes are sent.";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            Trace.TraceError($"{LoggerId}{msg}");
            return sent;
        }
        catch (Exception exception)
        {
            Trace.TraceError($"{LoggerId}{exception}");
            _duplexIoNoDataDelegate();
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return 0;
        }
    }

    private async Task<(int sent, bool isSent)> SendMessageInternal(IDataMessagingConfig dataMessagingConfig, DuplexIoNoDataDelegate duplexIoNoDataDelegate, IOutboundMessage message)
    {
        int sent;
        bool isSent;
        try
        {
            sent = await dataMessagingConfig.SocketProxy!.Send(message.RawMessageData);
            duplexIoNoDataDelegate();

            if (sent == 0)
            {
                //Trace.TraceInformation(
                //    $"Y:Sent: {sent} // {isSent}  // {message.RawMessageData.Length} bytes // {ArrayHelper.GetStringFromArrayCsharpStyle(message.RawMessageData)}");
                return (sent, false);
            }

            var s = $"{message.RawMessageDataClearText}  {message.ToShortInfoString()}";
            //Trace.TraceInformation(s);
            dataMessagingConfig.MonitorLogger.LogInformation($"Message sent: {s}");

            AsyncHelper.FireAndForget(() => dataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData));

            isSent = true;
            //Trace.TraceInformation($"Z:Sent: {sent} // {isSent}");
        }
        catch (SocketException socketException)
        {
            Trace.TraceError($"{LoggerId}{socketException}");
            AsyncHelper.FireAndForget(() =>
            {
                dataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("IpDuplexIoSender");
                dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message);
            });
            throw;
        }
        catch (Exception sendException)
        {
            Trace.TraceError($"{LoggerId}{sendException}");
            AsyncHelper.FireAndForget(() => dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, sendException.Message));
            isSent = false;
            sent = 0;
            //Trace.TraceInformation($"X: Sent: {sent} // {isSent}");
        }

        return (sent, isSent);
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