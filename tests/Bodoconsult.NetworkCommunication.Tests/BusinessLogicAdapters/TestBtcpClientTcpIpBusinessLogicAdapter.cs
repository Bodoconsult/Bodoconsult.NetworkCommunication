// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;

namespace Bodoconsult.NetworkCommunication.Tests.BusinessLogicAdapters
{
    /// <summary>
    /// Current adapter for TCP/IP channel from backend to client
    /// </summary>
    public class TestBtcpClientTcpIpBusinessLogicAdapter : BaseBtcpSimpleDeviceBusinessLogicAdapter
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
        public TestBtcpClientTcpIpBusinessLogicAdapter(IIpDevice device,
            IBusinessTransactionManager businessTransactionManager,
            IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter,
            IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter,
            IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter,
            IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter) :
            base(device, businessTransactionManager, inboundDataMessageToBtRequestConverter,
                inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter)
        { }
    }
}
