// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions.Adapters;

/// <summary>
/// Current implementation of <see cref="IInboundDataMessageToBusinessTransactionAdapter"/>
/// </summary>
public class InboundBtcpMessageToBusinessTransactionAdapter : IInboundBtcpMessageToBusinessTransactionAdapter
{

    public InboundBtcpMessageToBusinessTransactionAdapter(IOrderManagementDevice device,
        IBusinessTransactionManager businessTransactionManager,
        IInboundBtcpMessageToBtRequestDataConverter dataMessageConverter,
        IBtReplyToOutboundDataMessageConverter outboundDataMessageConverter,
        IAppLoggerProxy appLoggerProxy)
    {
        ArgumentNullException.ThrowIfNull(device.CommunicationAdapter);

        Device = device;
        BusinessTransactionManager = businessTransactionManager;
        InboundDataMessageConverter = dataMessageConverter;
        OutboundDataMessageConverter = outboundDataMessageConverter;
        AppLoggerProxy = appLoggerProxy;
    }

    public IOrderManagementDevice Device { get; }

    /// <summary>
    /// Current logger instance
    /// </summary>
    public IAppLoggerProxy AppLoggerProxy { get; }

    /// <summary>
    /// Current business transaction manager instance
    /// </summary>
    public IBusinessTransactionManager BusinessTransactionManager { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundBtcpMessageToBtRequestDataConverter"/> to convert an inbound data message to a business transaction request
    /// </summary>
    public IInboundBtcpMessageToBtRequestDataConverter InboundDataMessageConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction request to an outbound data message
    /// </summary>
    public IBtReplyToOutboundDataMessageConverter OutboundDataMessageConverter { get; }

    /// <summary>
    /// Handle an async received BTCP message
    /// </summary>
    public HandleAsyncBtcpMessageDelegate? HandleAsyncBtcpMessageDelegate { get; set; }

    /// <summary>
    /// Bind a received message to a business transaction and then execute it
    /// </summary>
    /// <param name="message">Received message</param>
    public void OnDataMessageReceived(IInboundDataMessage message)
    {
        ArgumentNullException.ThrowIfNull(Device.CommunicationAdapter);

        // Request data is required always!
        if (message is not IInboundBusinessTransactionDataMessage btm)
        {
            // Message is thrown away
            return;
        }

        // BT request?
        if (btm.IsRequest)
        {
            HandleRequestMessage(btm);
        }

        // BT reply
        HandleAsyncBtcpMessageDelegate?.Invoke(btm);
    }

    private void HandleRequestMessage(IInboundBusinessTransactionDataMessage btm)
    {
        ArgumentNullException.ThrowIfNull(Device.CommunicationAdapter);

        var internalRequest = InboundDataMessageConverter.MapToBusinessTransactionRequestData(btm);

        if (internalRequest == null)
        {
            AppLoggerProxy.LogError($"Received message is not a business transaction message: {btm.ToInfoString()}");
            return;
        }

        // Now execute the BT and then send a reply
        Task.Factory.StartNew(() => BusinessTransactionManager.RunBusinessTransaction(internalRequest.TransactionId, internalRequest))
            .ContinueWith(previousTask =>
            {
                try
                {
                    var reply = previousTask.Result;
                    var replyMessage = OutboundDataMessageConverter.MapToOutboundDataMessage(reply);

                    Device.CommunicationAdapter.SendDataMessage(replyMessage);
                }
                catch (Exception e)
                {
                    AppLoggerProxy.LogError(e, "Creating reply failed");
                }

            });
    }
}