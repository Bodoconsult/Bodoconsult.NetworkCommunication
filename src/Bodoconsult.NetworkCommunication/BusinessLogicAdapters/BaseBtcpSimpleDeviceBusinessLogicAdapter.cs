// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Abstractions.SyncExecution;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.BusinessLogicAdapters;

/// <summary>
/// Base class for BTCP based <see cref="ISimpleDeviceBusinessLogicAdapter"/> implementations
/// </summary>
public abstract class BaseBtcpSimpleDeviceBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IBtcpSimpleDeviceBusinessLogicAdapter
{
    private readonly ISyncProcessManager<Guid, IBusinessTransactionReply> _syncProcessManager = new SyncProcessManager<Guid, IBusinessTransactionReply>();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    /// <param name="inboundDataMessageToBtRequestConverter">Current converter for inbound data messages to BT requests</param>
    /// <param name="inboundDataMessageToBtReplyConverter">Current converter for inbound data messages to BT replies</param>
    /// <param name="outboundBtRequestToOutboundDataMessageConverter">Current converter for BT requests to outbound data messages</param>
    /// <param name="outboundBtReplyDataMessageConverter">Current converter for BT replies to outbound data messages</param>
    protected BaseBtcpSimpleDeviceBusinessLogicAdapter(IIpDevice device,
        IBusinessTransactionManager businessTransactionManager,
        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter,
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter,
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter,
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter) : base(device)
    {
        ArgumentNullException.ThrowIfNull(device.CommunicationAdapter);
        BusinessTransactionManager = businessTransactionManager;
        InboundDataMessageToBtRequestConverter = inboundDataMessageToBtRequestConverter;
        InboundDataMessageToBtReplyConverter = inboundDataMessageToBtReplyConverter;
        OutboundBtRequestToOutboundDataMessageConverter = outboundBtRequestToOutboundDataMessageConverter;
        OutboundBtReplyToOutboundDataMessageConverter = outboundBtReplyDataMessageConverter;
        AppLoggerProxy = device.DataMessagingConfig.AppLogger;
    }

    /// <summary>
    /// Current logger instance
    /// </summary>
    public IAppLoggerProxy AppLoggerProxy { get; }

    /// <summary>
    /// Current business transaction manager instance
    /// </summary>
    public IBusinessTransactionManager BusinessTransactionManager { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundMessageToBtRequestDataConverter"/> to convert an inbound data message to a business transaction request
    /// </summary>
    public IInboundMessageToBtRequestDataConverter InboundDataMessageToBtRequestConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IInboundDataMessageToBtReplyConverter"/> to convert an inbound data message to a business transaction reply
    /// </summary>
    public IInboundDataMessageToBtReplyConverter InboundDataMessageToBtReplyConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction request to an outbound data message
    /// </summary>
    public IBtReplyToOutboundDataMessageConverter OutboundBtReplyToOutboundDataMessageConverter { get; }

    /// <summary>
    /// Current instance of <see cref="IBtReplyToOutboundDataMessageConverter"/> to convert a business transaction request to an outbound data message
    /// </summary>
    public IBtRequestDataToOutboundDataMessageConverter OutboundBtRequestToOutboundDataMessageConverter { get; }

    /// <summary>
    /// Send a business transaction request to the device
    /// </summary>
    /// <param name="request">Current BT request</param>
    /// <returns>Message sending result</returns>
    public MessageSendingResult SendBtRequest(IBusinessTransactionRequestData request)
    {
        var message = OutboundBtRequestToOutboundDataMessageConverter.MapToOutboundDataMessage(request);

        if (message == null)
        {
            return MessageSendingResultHelper.Error("No outbound message was created");
        }

        // ToDo: replace timeout with request.Timeout
        var syncData = _syncProcessManager.AddSyncProcess(request.TransactionGuid, 5000);
        syncData.BusinessTransactionRequestData = request;

        // Now wait for order execution (doing it in a non-blocking mannor)
        var unused = AsyncHelper.RunSync(syncData.CreateWaitingTask);

        // Remove the order from waiting queue
        _syncProcessManager.RemoveSyncProcess(request.TransactionGuid);

        return SendMessage(message);
    }

    /// <summary>
    /// Send a business transaction request as message to the device without waiting for answer
    /// </summary>
    /// <param name="message">Current BT request message</param>
    /// <returns>Message sending result</returns>
    public MessageSendingResult SendBtRequestNotWaitingForAnswer(BtcpRequestOutboundDataMessage message)
    {
        return SendMessage(message);
    }

    /// <summary>
    /// Send a business transaction request to the device
    /// </summary>
    /// <param name="reply">Current BT reply</param>
    /// <returns>Message sending result</returns>
    public MessageSendingResult SendBtReply(IBusinessTransactionReply reply)
    {
        var message = OutboundBtReplyToOutboundDataMessageConverter.MapToOutboundDataMessage(reply);

        if (message == null)
        {
            return MessageSendingResultHelper.Error("No outbound message was created");
        }

        return SendMessage(message);
    }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        // Request data is required always!
        if (message is BtcpRequestInboundDataMessage request)
        {
            HandleRequestMessage(request);
        }
        else if (message is BtcpReplyInboundDataMessage reply)
        {
            HandleReplyMessage(reply);
        }
    }

    private void HandleReplyMessage(IInboundBusinessTransactionDataMessage btm)
    {
        var reply = InboundDataMessageToBtReplyConverter.MapToBusinessTransactionReply(btm);

        if (reply == null)
        {
            return;
        }

        

        var syncData = _syncProcessManager.GetSyncProcessDataForProcess(reply.RequestData.TransactionGuid);

        if (syncData == null)
        {
            return;
        }

        reply.RequestData = syncData.BusinessTransactionRequestData;
        syncData.TaskCompletionSource?.SetResult(reply);

        Trace.TraceInformation($"BaseBtcpSimpleDeviceBusinessLogicAdapter: reply for BT {reply.RequestData.TransactionId}");
    }

    private void HandleRequestMessage(BtcpRequestInboundDataMessage btm)
    {
        var internalRequest = InboundDataMessageToBtRequestConverter.MapToBusinessTransactionRequestData(btm);

        if (internalRequest == null)
        {
            AppLoggerProxy.LogError($"Received message is not a business transaction message: {btm.ToInfoString()}");
            return;
        }

        Trace.TraceInformation($"BaseBtcpSimpleDeviceBusinessLogicAdapter: request for BT {internalRequest.TransactionId}");

        // Now execute the BT and then send a reply
        Task.Factory.StartNew(() => BusinessTransactionManager.RunBusinessTransaction(internalRequest.TransactionId, internalRequest))
            .ContinueWith(previousTask =>
            {
                try
                {
                    var reply = previousTask.Result;
                    var replyMessage = OutboundBtReplyToOutboundDataMessageConverter.MapToOutboundDataMessage(reply);

                    if (replyMessage == null)
                    {
                        return;
                    }

                    SendMessage(replyMessage);
                }
                catch (Exception e)
                {
                    AppLoggerProxy.LogError(e, "Creating reply failed");
                }
            });
    }
}