// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpClient.Bll.Interfaces;

/// <summary>
/// Interface for the client UI management
/// </summary>
public interface IClientUiManager
{
    /// <summary>
    /// Current IP config of the backend for TCP/IP
    /// </summary>
    public IpConfig? BackendTcpIpConfig { get; set; }

    /// <summary>
    /// Represents the TCP/IP communication with the IP device
    /// </summary>
    IOrderManagementDeviceManager? BackendTcpIp { get; }

    /// <summary>
    /// Load the comm via TCP/IP to the backend
    /// </summary>
    void LoadBackendTcpIp();

    /// <summary>
    /// Load the business transactions required for the app
    /// </summary>
    void LoadBusinessTransactions();

    /// <summary>
    /// Start the communication with the TCP/IP backend
    /// </summary>
    void StartBackendTcpIpCommunication();
}