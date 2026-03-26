// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for BTCP implementation of <see cref="ISimpleDeviceBusinessLogicAdapter"/>
/// </summary>
public interface IBtcpSimpleDeviceBusinessLogicAdapter: ISimpleDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current logger instance
    /// </summary>
    IAppLoggerProxy AppLoggerProxy { get; }

    /// <summary>
    /// Current business transaction manager instance
    /// </summary>
    IBusinessTransactionManager BusinessTransactionManager { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundMessageToBtRequestDataConverter"/> to convert an inbound data message to a business transaction request
    /// </summary>
    IInboundMessageToBtRequestDataConverter InboundDataMessageToBtRequestConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundDataMessageToBtReplyConverter"/> to convert an inbound data message to a business transaction reply
    /// </summary>
    IInboundDataMessageToBtReplyConverter InboundDataMessageToBtReplyConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction request to an outbound data message
    /// </summary>
    IBtReplyToOutboundDataMessageConverter OutboundBtReplyToOutboundDataMessageConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction request to an outbound data message
    /// </summary>
    IBtRequestDataToOutboundDataMessageConverter OutboundBtRequestToOutboundDataMessageConverter { get; }

    /// <summary>
    /// Send a business transaction request to the device
    /// </summary>
    /// <param name="request">Current BT request</param>
    /// <returns>Message sending result</returns>
    MessageSendingResult SendBtRequest(IBusinessTransactionRequestData request);

    /// <summary>
    /// Send a business transaction request to the device
    /// </summary>
    /// <param name="reply">Current BT reply</param>
    /// <returns>Message sending result</returns>
    MessageSendingResult SendBtReply(IBusinessTransactionReply reply);
}