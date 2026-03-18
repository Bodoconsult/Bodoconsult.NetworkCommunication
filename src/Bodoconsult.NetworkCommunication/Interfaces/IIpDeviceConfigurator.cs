// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for <see cref="IIpDevice"/> configurators setting up the device with basic settings
/// </summary>
public interface IIpDeviceConfigurator
{
    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IIpDataMessagingConfig? DataMessagingConfig { get; }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice? Device { get; }

    /// <summary>
    /// Create the basic data messaging config
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    void CreateMessagingConfig(string ipAddress, int port);

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    void CreateDevice();

    /// <summary>
    /// Configure the order management and if necessary the state management
    /// </summary>
    void ConfigureOrderManagementAndStateManagement();

    /// <summary>
    /// Get the read configured device
    /// </summary>
    /// <returns>Freshly created and configured <see cref="IIpDevice"/> instance</returns>
    IIpDevice GetDevice();
}