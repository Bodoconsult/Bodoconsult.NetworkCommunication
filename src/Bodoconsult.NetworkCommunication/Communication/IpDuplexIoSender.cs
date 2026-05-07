// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net.Sockets;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

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
        string msg;

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

                    DataMessagingConfig.MonitorLogger.LogInformation($"Sent: {sent} // {isSent}");

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
            
            msg = "message could not be sent via socket";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            return sent;
        }
        catch (Exception exception)
        {
            msg = "message could not be sent via socket";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            _duplexIoNoDataDelegate();
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return 0;
        }
    }

    private async Task<(int sent, bool isSent)> SendMessageInternal(IDataMessagingConfig dataMessagingConfig, DuplexIoNoDataDelegate duplexIoNoDataDelegate, IOutboundMessage message)
    {
        string msg;
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

            msg = $"{message.RawMessageDataClearText}  {message.ToShortInfoString()}";
            dataMessagingConfig.MonitorLogger.LogInformation($"Message sent: {msg}");

            AsyncHelper.FireAndForget(() =>
            {
                try
                {
dataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            });

            isSent = true;
            //Trace.TraceInformation($"Z:Sent: {sent} // {isSent}");
        }
        catch (SocketException socketException)
        {
            msg = $"message {message.ToShortInfoString()} not sent: {socketException}";
            dataMessagingConfig.MonitorLogger.LogInformation(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

            AsyncHelper.FireAndForget(() =>
            {
                dataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("IpDuplexIoSender");
                dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message);
            });
            throw;
        }
        catch (Exception sendException)
        {
            msg = $"message {message.ToShortInfoString()} not sent: {sendException}";
            dataMessagingConfig.MonitorLogger.LogInformation(msg);
            dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

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