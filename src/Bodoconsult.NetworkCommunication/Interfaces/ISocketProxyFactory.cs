// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for socket based communication proxies
/// </summary>
public interface ISocketProxyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ISocketProxy"/>
    /// </summary>
    /// <param name="isServer">Is the device configured as IP server. True = server, false = client. Default: false</param>
    /// <param name="protocol">IP base protocol to be used</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <param name="monitorlogger"></param>
    /// <returns>Instance of <see cref="ISocketProxy"/></returns>
    ISocketProxy CreateInstance(bool isServer, IpProtocolEnum protocol, IPAddress ipAddress, int port,
        IAppLoggerProxy monitorlogger);
}