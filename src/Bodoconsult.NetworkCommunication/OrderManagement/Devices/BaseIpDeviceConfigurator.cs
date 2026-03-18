// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// Base class for device configurators
/// </summary>
public abstract class BaseIpDeviceConfigurator : IIpDeviceConfigurator
{
    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IIpDataMessagingConfig? DataMessagingConfig { get; protected set; }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice? Device { get; protected set; }

    /// <summary>
    /// Create the basic data messaging config
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public virtual void CreateMessagingConfig(string ipAddress, int port)
    {
        throw new NotSupportedException("Override in derived classes!");
    }

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    public virtual void CreateDevice()
    {
        throw new NotSupportedException("Override in derived classes!");
    }

    /// <summary>
    /// Configure the order management and if necessary the state management
    /// </summary>
    public virtual void ConfigureOrderManagementAndStateManagement()
    {
        throw new NotSupportedException("Override in derived classes!");
    }

    /// <summary>
    /// Get the read configured device
    /// </summary>
    /// <returns>Freshly created and configured <see cref="IIpDevice"/> instance</returns>
    public IIpDevice GetDevice()
    {
        ArgumentNullException.ThrowIfNull(Device, "Create and configure the device with other methods before");
        return Device;
    }
}