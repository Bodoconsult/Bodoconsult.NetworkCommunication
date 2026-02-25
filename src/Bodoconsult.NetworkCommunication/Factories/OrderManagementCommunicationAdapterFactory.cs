// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderManagementCommunicationAdapterFactory"/>
/// </summary>
public class OrderManagementCommunicationAdapterFactory : IOrderManagementCommunicationAdapterFactory
{
    private readonly ICommunicationHandlerFactory _communicationHandlerFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderManagementCommunicationAdapterFactory(ICommunicationHandlerFactory smdTowerCommunicationHandlerFactory)
    {
        _communicationHandlerFactory = smdTowerCommunicationHandlerFactory;
    }

    /// <summary>
    /// Create an instance implementing <see cref="OrderManagementCommunicationAdapter"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="OrderManagementCommunicationAdapter"/></returns>
    public IOrderManagementCommunicationAdapter CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        if (dataMessagingConfig == null)
        {
            throw new NullReferenceException("dataMessagingConfig may not be NULL");
        }

        return new OrderManagementCommunicationAdapter(dataMessagingConfig, _communicationHandlerFactory);
    }
}