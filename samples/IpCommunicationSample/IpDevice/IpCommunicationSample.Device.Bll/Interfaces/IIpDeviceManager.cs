// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Device.Bll.Interfaces;

public interface IIpDeviceManager
{
    /// <summary>
    /// Current IP config of the backend for TCP/IP
    /// </summary>
    public IpConfig? BackendTcpIpConfig { get; set; }

    /// <summary>
    /// Current IP config of the backend for UDP
    /// </summary>
    public IpConfig? BackendUdpConfig { get; set; }

    /// <summary>
    /// Represents the TCP/IP communication with the backend
    /// </summary>
    ISimpleDeviceManager? BackendTcpIp { get; }

    /// <summary>
    /// Represents the UDP communication with the backend
    /// </summary>
    ISimpleDeviceManager? BackendUdp { get; }

    /// <summary>
    /// Load the comm via TCP/IP to the backend
    /// </summary>
    void LoadBackendTcpIp();

    /// <summary>
    /// Load the comm via UDP to the dbackend
    /// </summary>
    void LoadBackendUdp();
}