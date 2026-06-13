// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message sending version 1 for IP based networks (UDP/TCP)
/// </summary>
public class IpDuplexIoSender : BaseDuplexIoSender
{

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current device comm settings</param>
    public IpDuplexIoSender(IDataMessagingConfig dataMessagingConfig) : base(dataMessagingConfig)
    { }

    ///// <summary>
    ///// Start the message sender
    ///// </summary>
    //public override async Task StartSender()
    //{
    //    // Do nothing
    //    await Task.Run(() => { });
    //}

    ///// <summary>
    ///// Stop the message sender
    ///// </summary>
    //public override async Task StopSender()
    //{
    //    // Do nothing
    //    await Task.Run(() => { });
    //}


    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public override async Task<MessageSendingResult> SendMessage(IOutboundMessage message)
    {
        string msg;

        ArgumentNullException.ThrowIfNull(DataMessagingConfig.SocketProxy);

        var i = 0;


        if (EncodeMessage(message))
        {
            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }

        // Send message
        try
        {
            //Create a byte array from message according to current protocol
            while (i < 3)
            {
                var msr = await SendMessageInternal(message);

                if (msr.ProcessExecutionResult == OrderExecutionResultState.Successful)
                {
                    //#if DEBUG
                    DataMessagingConfig.MonitorLogger.LogInformation($"{LoggerId}Sent: {message.ToInfoString()}");
                    //#endif
                    return msr;
                }

                AsyncHelper.Delay(10);
                i++;
            }

            msg = "message could not be sent via socket";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }
        catch (Exception exception)
        {
            msg = "message could not be sent via socket";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }
    }

    //private static async Task<(int sent, bool isSent)> SendMessageInternal(IDataMessagingConfig dataMessagingConfig, IOutboundMessage message)
    //{
    //    string msg;
    //    int sent;
    //    bool isSent;
    //    try
    //    {
    //        sent = await dataMessagingConfig.SocketProxy!.Send(message.RawMessageData);

    //        if (sent == 0)
    //        {
    //            //Trace.TraceInformation(
    //            //    $"Y:Sent: {sent} // {isSent}  // {message.RawMessageData.Length}B// {ArrayHelper.GetStringFromArrayCsharpStyle(message.RawMessageData)}");
    //            return (sent, false);
    //        }

    //        //msg = $"{message.RawMessageDataClearText}  {message.ToShortInfoString()}";
    //        //dataMessagingConfig.MonitorLogger.LogInformation($"Message sent: {msg}");

    //        AsyncHelper.FireAndForget(() =>
    //        {
    //            try
    //            {
    //                dataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData);
    //            }
    //            catch (Exception e)
    //            {
    //                msg = $"message could not be sent via socket: {e}";
    //                dataMessagingConfig.MonitorLogger.LogError(msg);
    //                dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
    //            }
    //        });

    //        isSent = true;
    //        //Trace.TraceInformation($"Z:Sent: {sent} // {isSent}");
    //    }
    //    catch (SocketException socketException)
    //    {
    //        msg = $"message {message.ToShortInfoString()} not sent: {socketException}";
    //        dataMessagingConfig.MonitorLogger.LogError(msg);
    //        dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

    //        AsyncHelper.FireAndForget(() =>
    //        {
    //            try
    //            {
    //                dataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("IpDuplexIoSender");
    //                dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData,
    //                    socketException.Message);
    //            }
    //            catch (Exception e)
    //            {
    //                msg = $"firing delegates failed: {e}";
    //                dataMessagingConfig.MonitorLogger.LogError(msg);
    //                dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
    //            }
    //        });
    //        isSent = false;
    //        sent = 0;
    //    }
    //    catch (Exception sendException)
    //    {
    //        msg = $"message {message.ToShortInfoString()} not sent: {sendException}";
    //        dataMessagingConfig.MonitorLogger.LogError(msg);
    //        dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");

    //        AsyncHelper.FireAndForget(() =>
    //        {
    //            try
    //            {
    //                dataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData,
    //                    sendException.Message);
    //            }
    //            catch (Exception e)
    //            {
    //                msg = $"dataMessagingConfig.RaiseDataMessageNotSentDelegate failed: {e}";
    //                dataMessagingConfig.MonitorLogger.LogError(msg);
    //                dataMessagingConfig.AppLogger.LogError($"{dataMessagingConfig.LoggerId}{msg}");
    //            }

    //        });
    //        isSent = false;
    //        sent = 0;
    //        //Trace.TraceInformation($"X: Sent: {sent} // {isSent}");
    //    }

    //    return (sent, isSent);
    //}

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