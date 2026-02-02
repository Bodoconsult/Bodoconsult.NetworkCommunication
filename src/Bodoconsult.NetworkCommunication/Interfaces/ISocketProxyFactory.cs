// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for socket based communication proxies
/// </summary>
public interface ISocketProxyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    ISocketProxy CreateInstance();

}