// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP channel from backend to client
/// </summary>
public class BtcpClientTcpIpBusinessLogicAdapter : BaseBtcpSimpleDeviceBusinessLogicAdapter, IClientTcpIpDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current IP device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    /// <param name="inboundDataMessageToBtRequestConverter">Current converter for inbound data messages to BT requests</param>
    /// <param name="inboundDataMessageToBtReplyConverter">Current converter for inbound data messages to BT replies</param>
    /// <param name="outboundBtRequestToOutboundDataMessageConverter">Current converter for BT requests to outbound data messages</param>
    /// <param name="outboundBtReplyDataMessageConverter">Current converter for BT replies to outbound data messages</param>
    public BtcpClientTcpIpBusinessLogicAdapter(IIpDevice device,
        IBusinessTransactionManager businessTransactionManager,
        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter,
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter,
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter,
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter) : 
            base(device, businessTransactionManager, inboundDataMessageToBtRequestConverter, 
            inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter)
    {}


    /// <summary>
    /// Report an error reported by the device to the client
    /// </summary>
    /// <param name="request">Error request</param>
    /// <returns>Default transaction reply</returns>
    public IBusinessTransactionReply ReportDeviceError(IBusinessTransactionRequestData request)
    {
        if (request is not ErrorBusinessTransactionRequestData err)
        {
            throw new ArgumentException("request is not ErrorBusinessTransactionRequestData");
        }

        var message = OutboundBtRequestToOutboundDataMessageConverter.MapToOutboundDataMessage(err);

        if (message is not BtcpRequestOutboundDataMessage msg)
        {
            return new DefaultBusinessTransactionReply
            {
                ErrorCode = 1,
                Message = "Request was not converted to a message",
                RequestData = request
            };
        }

        var result = SendBtRequestNotWaitingForAnswer(msg);

        if (result.ProcessExecutionResult != OrderExecutionResultState.Successful)
        {
            return new DefaultBusinessTransactionReply
            {
                ErrorCode = 1,
                Message = "Message was not sent",
                ExceptionMessage = result.Information,
                RequestData = request
            };
        }
        return new DefaultBusinessTransactionReply
        {
            ErrorCode = 0,
            RequestData = request
        };
    }
}