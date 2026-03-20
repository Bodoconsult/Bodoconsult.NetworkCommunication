// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Backend.Bll.Interfaces;

public interface IBackendManager
{
    /// <summary>
    /// Represents the TCP/IP communication with the client
    /// </summary>
    IOrderManagementDevice Client { get; }

    /// <summary>
    /// Represents the TCP/IP communication with the IP device
    /// </summary>
    IStateMachineDevice IpDeviceTcpIp { get; }

    /// <summary>
    /// Represents the UDP communication with the IP device
    /// </summary>
    IOrderManagementDevice IpDeviceUdp { get; }
}