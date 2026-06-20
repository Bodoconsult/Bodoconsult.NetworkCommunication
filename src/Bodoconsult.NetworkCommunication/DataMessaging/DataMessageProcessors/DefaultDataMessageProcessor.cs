// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for plain data messages. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class DefaultDataMessageProcessor : BaseDataMessageProcessor
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public DefaultDataMessageProcessor(IDataMessagingConfig config) : base(config)
    { }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        var s = $"received {message.ToInfoString()}";
        Config.MonitorLogger.LogInformation(s);

        Stopped.Reset();

        // Handshake received
        if (message is IInboundHandShakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // Data message received
        if (message is IInboundDataMessage dataMessage)
        {
            ProcessDataMessage(dataMessage);
            return;
        }

        // No valid message
        s = $"{message.ToShortInfoString()}: not valid: {message.GetType().Name}: {message.RawMessageDataClearText}";
        Config.MonitorLogger.LogError(s);
    }

    private void ProcessDataMessage(IInboundDataMessage dataMessage)
    {
        ArgumentNullException.ThrowIfNull(Config.RaiseCommLayerDataMessageReceivedDelegate);

        //Config.RaiseCommLayerDataMessageReceivedDelegate.Invoke(dataMessage);
        //return;

        // Now process the message
        Config.RaiseCommLayerDataMessageReceivedDelegate.Invoke(dataMessage);

        //AsyncHelper.FireAndForget2(() =>
        //{
        //    try
        //    {
        //        Config.RaiseCommLayerDataMessageReceivedDelegate.Invoke(dataMessage);
        //    }
        //    catch (Exception e)
        //    {
        //        var s = $" failed {dataMessage.ToShortInfoString()}: {e}";
        //        Config.MonitorLogger.LogError(s);
        //    }
        //}).ContinueWith(Callback);

        //var result = Stopped.WaitOne(TimeOut);
        //if (result)
        //{
        //    return;
        //}
        //var msg = $"{dataMessage.ToShortInfoString()}: delivering to receiver timed out";
        //Config.AppLogger.LogError($"{Config.LoggerId}{msg}");
        //Config.MonitorLogger.LogError(msg);
    }
}