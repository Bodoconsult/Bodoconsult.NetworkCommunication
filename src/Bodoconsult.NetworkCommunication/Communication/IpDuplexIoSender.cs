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

            msg = "message not sent";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }
        catch (Exception exception)
        {
            msg = "message not sent";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
        }
    }

    
    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected override async Task Dispose(bool disposing)
    {
        //if (!disposing)
        //{
        //}

        await Task.Run(() => { });
    }
}