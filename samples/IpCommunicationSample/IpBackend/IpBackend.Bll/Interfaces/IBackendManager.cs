// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSample.Backend.Bll.Interfaces;

public interface IBackendManager
{
    /// <summary>
    /// Current IP config of the IP device for TCP/IP
    /// </summary>
    public IpConfig? IpDeviceTcpIpConfig { get; set; }

    /// <summary>
    /// Current IP config of the IP device for UDP
    /// </summary>
    public IpConfig? IpDeviceUdpConfig { get; set; }

    /// <summary>
    /// Current IP config of the client for TCP/IP
    /// </summary>
    public IpConfig? ClientTcpIpConfig { get; set; }
    
    /// <summary>
    /// Represents the TCP/IP communication with the client
    /// </summary>
    IIpDevice? Client { get; }

    /// <summary>
    /// Represents the TCP/IP communication with the IP device
    /// </summary>
    IStateMachineDevice? IpDeviceTcpIp { get; }

    /// <summary>
    /// Represents the UDP communication with the IP device
    /// </summary>
    IOrderManagementDevice? IpDeviceUdp { get; }

    /// <summary>
    /// Load the comm via TCP/IP to the device
    /// </summary>
    void LoadIpDeviceTcpIp();

    /// <summary>
    /// Load the comm via UDP to the device
    /// </summary>
    void LoadIpDeviceUdp();

    /// <summary>
    /// Load the client
    /// </summary>
    void LoadClient();
}