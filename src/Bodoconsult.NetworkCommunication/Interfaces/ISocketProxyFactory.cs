// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.NetworkCommunication.EnumAndStates;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for socket based communication proxies
/// </summary>
public interface ISocketProxyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <param name="protocol">IP base protocol to be used</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    ISocketProxy CreateInstance(IpProtocolEnum protocol, IPAddress ipAddress, int port);
}