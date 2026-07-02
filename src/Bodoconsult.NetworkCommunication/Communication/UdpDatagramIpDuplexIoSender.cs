// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message sending version 1 for IP based networks for UDP only
/// </summary>
public class UdpDatagramIpDuplexIoSender : BaseDuplexIoSender
{
    private ulong _sendCounter;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current device comm settings</param>
    public UdpDatagramIpDuplexIoSender(IDataMessagingConfig dataMessagingConfig) : base(dataMessagingConfig)
    { }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public override async Task<MessageSendingResult> SendMessage(IOutboundMessage message)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.SocketProxy);

        string msg;
        try
        {
            //Trace.TraceInformation("Send really");

            if (EncodeMessage(message))
            {
                return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
            }

            // Send message
            var sent = await SendMessageInternal(message);
            if (_sendCounter == ulong.MaxValue)
            {
                _sendCounter = 0;
            }

            _sendCounter++;

            if (_sendCounter % 100 == 0)
            {
                DataMessagingConfig.MonitorLogger.LogInformation($"sent {_sendCounter} messages");
            }

            return sent;
        }
        catch (Exception e)
        {
            msg = $"{message.ToShortInfoString()} could not be sent via UDP socket: {e}";
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, msg));
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");

            return new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful);
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