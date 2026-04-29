// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Base class for <see cref="IDataMessageProcessor"/> implementations
/// </summary>
public abstract class BaseDataMessageProcessor : IDataMessageProcessor
{
    /// <summary>
    /// Logger ID
    /// </summary>
    protected readonly string LoggerId;

    /// <summary>
    /// Stopped event to wait for messages to be delivered to next step
    /// </summary>
    protected readonly AutoResetEvent Stopped = new(false);

    /// <summary>
    /// Timeout in ms for waiting for messages to be delivered to next step
    /// </summary>
    protected const int TimeOut = 2000;

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseDataMessageProcessor(IDataMessagingConfig config)
    {
        Config = config;
        LoggerId = $"{config.LoggerId}{(config.LoggerId.EndsWith(": ") ? string.Empty : ": ")}{GetType().Name}: ";
    }

    /// <summary>
    /// Current <see cref="IDataMessagingConfig"/> instance
    /// </summary>
    public readonly IDataMessagingConfig Config;

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public virtual void ProcessMessage(IInboundMessage message)
    {
        throw new NotSupportedException("Override method in derived classes");
    }

    /// <summary>
    /// Process handshakes
    /// </summary>
    /// <param name="handShake">Handshake message</param>
    // ReSharper disable once SuggestBaseTypeForParameter
    protected void ProcessHandshakes(IInboundHandShakeMessage handShake)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);

        // fire and forget but let CallBack() be run at the end
        AsyncHelper.FireAndForget2(() => Config.DataMessageProcessingPackage.WaitStateManager.OnHandshakeReceived(handShake))
            .ContinueWith(Callback);
        Stopped.WaitOne(TimeOut);
        //Config.MonitorLogger?.LogInformation($"received handshake message [{hs.HandshakeMessageType:X2}]");

    }

    /// <summary>
    /// Callback metho th free <see cref="Stopped"/>
    /// </summary>
    /// <param name="ar">Asny result (not handled)</param>
    protected void Callback(IAsyncResult ar)
    {
        Stopped.Set();
    }
}