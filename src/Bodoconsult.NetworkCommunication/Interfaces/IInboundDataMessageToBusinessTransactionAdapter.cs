// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Adapter to bind receiving data messages to business transactions
/// </summary>
public interface IInboundDataMessageToBusinessTransactionAdapter
{
    /// <summary>
    /// Current device
    /// </summary>
    IIpDevice Device { get; }

    /// <summary>
    /// Current logger instance
    /// </summary>
    IAppLoggerProxy AppLoggerProxy { get; }

    /// <summary>
    /// Current business transaction manager instance
    /// </summary>
    IBusinessTransactionManager BusinessTransactionManager { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundDataMessageToBtReplyConverter"/> to convert an inbound data message to a business transaction request
    /// </summary>
    IInboundDataMessageToBtReplyConverter InboundDataMessageConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction reply to an outbound data message
    /// </summary>
    IBtReplyToOutboundDataMessageConverter OutboundDataMessageConverter { get; }

    /// <summary>
    /// Bind a received message to a business transaction and then execute it
    /// </summary>
    /// <param name="message">Received message</param>
    void OnDataMessageReceived(IInboundDataMessage message);
}