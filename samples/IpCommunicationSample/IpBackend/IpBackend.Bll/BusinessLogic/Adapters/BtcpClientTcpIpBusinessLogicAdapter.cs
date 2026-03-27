// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

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
}