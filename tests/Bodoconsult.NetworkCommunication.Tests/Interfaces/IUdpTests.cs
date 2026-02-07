// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Testing;

namespace Bodoconsult.NetworkCommunication.Tests.Interfaces;

public interface IUdpTests
{
    /// <summary>
    /// Current UDP remote device to send data to the socket
    /// </summary>
    IUdpDevice RemoteUdpDevice { get; set; }

    /// <summary>
    /// Current IP address to use
    /// </summary>
    IPAddress IpAddress { get; set; }

    /// <summary>
    /// Device communication data
    /// </summary>
    IIpDataMessagingConfig DataMessagingConfig { get; set; }

    /// <summary>
    /// General log file
    /// </summary>
    IAppLoggerProxy Logger { get; set; }

    /// <summary>
    /// Current <see cref="ISocketProxy"/> implementation to use
    /// </summary>
    ISocketProxy Socket { get; set; }

}