// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="ICommunicationAdapterFactory"/>
/// </summary>
public class IpCommunicationAdapterFactory : ICommunicationAdapterFactory
{
    private readonly ICommunicationHandlerFactory _communicationHandlerFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpCommunicationAdapterFactory(ICommunicationHandlerFactory communicationHandlerFactory)
    {
        _communicationHandlerFactory = communicationHandlerFactory;
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

        if (dataMessagingConfig.DataMessageProcessingPackage?.OutboundDataMessageFactory == null)
        {
            throw new NullReferenceException("dataMessagingConfig.DataMessageProcessingPackage?.OutboundDataMessageFactory may not be NULL");
        }

        return new IpCommunicationAdapter(dataMessagingConfig, _communicationHandlerFactory);
    }
}