// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
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
    public override async Task<int> SendMessage(IOutboundDataMessage message)
    {

        var sent = 0;

        var i = 0;
        var isSent = false;

        OutboundCodecResult result;

        // Encode message
        try
        {
            result = DataMessageCodingProcessor.EncodeDataMessage(message);
                
            if (result.ErrorCode != 0)
            {
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, result.ErrorMessage));
                AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new Exception(result.ErrorMessage)));
                return 0;
            }
        }
        catch (Exception encodeException)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(null, encodeException.Message));
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(encodeException));
            return sent;
        }

        // Send message
        try
        {
            //Create a byte array from message according to current protocol
            while (i < 3)
            {
                if (!_duplexIoIsWorkInProgressDelegate())
                {

                    try
                    {
                        sent = await DataMessagingConfig.SocketProxy.Send(message.RawMessageData);  
                        _duplexIoNoDataDelegate();
                            
                        var s = $"{message.RawMessageDataClearText}  {message.ToShortInfoString()}";
                        Debug.Print(s);
                        DataMessagingConfig.MonitorLogger?.LogInformation($"Message sent: {s}");

                        AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData));
                    }
                    catch (SocketException socketException)
                    {
                        AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("TcpIpDuplexIoSender"));
                        AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, socketException.Message));
                        throw;
                    }
                    catch (Exception sendException)
                    {
                        AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, sendException.Message));
                        throw;
                    }
                        
                    isSent = true;
                    break;
                }

                AsyncHelper.Delay(10);
                i++;
            }

            if (!isSent)
            {
                DataMessagingConfig.MonitorLogger?.LogError($"send process blocked by another socket operation");
                return 0;
            }

            if (sent > 0)
            {
                return sent;
            }
            var msg = $"{DataMessagingConfig.LoggerId}message could not be sent via TCP socket. Only {0} bytes of {message.RawMessageData.Length} bytes are sent.";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger?.LogError(msg);
            DataMessagingConfig.AppLogger?.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            return sent;
        }
        catch (Exception exception)
        {
            _duplexIoNoDataDelegate();
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return 0;
        }
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