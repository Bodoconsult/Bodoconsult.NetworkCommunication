// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderManagementCommunicationAdapterFactory"/>
/// </summary>
public class IpCommunicationAdapterFactory : IOrderManagementCommunicationAdapterFactory
{
    private readonly ICommunicationHandlerFactory _communicationHandlerFactory;
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpCommunicationAdapterFactory(ICommunicationHandlerFactory communicationHandlerFactory, IOutboundDataMessageFactory outboundDataMessageFactory)
    {
        _communicationHandlerFactory = communicationHandlerFactory;
        _outboundDataMessageFactory = outboundDataMessageFactory;
    }

    /// <summary>
    /// Create an instance implementing <see cref="IpCommunicationAdapter"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="IpCommunicationAdapter"/></returns>
    public ICommunicationAdapter CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        if (dataMessagingConfig == null)
        {
            throw new NullReferenceException("dataMessagingConfig may not be NULL");
        }

        return new IpCommunicationAdapter(dataMessagingConfig, _communicationHandlerFactory, _outboundDataMessageFactory);
    }
}