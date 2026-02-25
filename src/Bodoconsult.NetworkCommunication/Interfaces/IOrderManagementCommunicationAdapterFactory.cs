// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories to create an instance of <see cref="OrderManagementCommunicationAdapter"/> implementations
/// </summary>
public interface IOrderManagementCommunicationAdapterFactory
{
    /// <summary>
    /// Create an instance implementing <see cref="OrderManagementCommunicationAdapter"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="IOrderManagementCommunicationAdapter"/></returns>
    IOrderManagementCommunicationAdapter CreateInstance(IIpDataMessagingConfig dataMessagingConfig);
}