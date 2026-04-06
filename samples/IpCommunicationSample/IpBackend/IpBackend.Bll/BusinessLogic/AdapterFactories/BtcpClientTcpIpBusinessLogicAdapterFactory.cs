// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.Adapters;
using IpBackend.Bll.BusinessLogic.Converters;

namespace IpBackend.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="BtcpClientTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class BtcpClientTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public BtcpClientTcpIpBusinessLogicAdapterFactory(IBusinessTransactionManager businessTransactionManager)
    {
        _businessTransactionManager = businessTransactionManager;
    }

    /// <summary>
    /// Create an instance of <see cref="ISimpleDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        var logger = device.DataMessagingConfig.AppLogger;

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new ClientInboundBtcpMessageToBtRequestDataConverter(logger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new ClientInboundBtcpMessageToBtReplyConverter(logger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new ClientBtRequestDataToOutboundBtcpMessageConverter(logger);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new ClientBtReplyToOutboundDataMessageConverter(logger);
        
        return new BtcpClientTcpIpBusinessLogicAdapter(device, _businessTransactionManager, 
            inboundDataMessageToBtRequestConverter, inboundDataMessageToBtReplyConverter, 
            outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);
    }
}