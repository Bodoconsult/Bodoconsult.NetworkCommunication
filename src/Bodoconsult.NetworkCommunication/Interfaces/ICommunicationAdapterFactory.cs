// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories to create an instance of <see cref="IpCommunicationAdapter"/> implementations
/// </summary>
public interface ICommunicationAdapterFactory
{
    /// <summary>
    /// Create an instance implementing <see cref="IpCommunicationAdapter"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="ICommunicationAdapter"/></returns>
    ICommunicationAdapter CreateInstance(IIpDataMessagingConfig dataMessagingConfig);
}