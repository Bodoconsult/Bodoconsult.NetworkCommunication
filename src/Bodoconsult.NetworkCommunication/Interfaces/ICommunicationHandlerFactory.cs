// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories to create an instance of <see cref="ICommunicationHandler"/> implementations
/// </summary>
public interface ICommunicationHandlerFactory
{
    /// <summary>
    /// Create an instance implementing <see cref="ICommunicationHandler"/>
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    /// <returns>An instance implementing <see cref="ICommunicationHandler"/></returns>
    ICommunicationHandler CreateInstance(IIpDataMessagingConfig dataMessagingConfig);
}