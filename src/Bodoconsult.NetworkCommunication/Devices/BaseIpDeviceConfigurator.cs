// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// Base class for device configurators
/// </summary>
public abstract class BaseIpDeviceConfigurator : IIpDeviceConfigurator
{
    #region Helper methods

    /// <summary>
    /// Create a monitor logger for the device
    /// </summary>
    /// <param name="monitorLoggerFactoryFactory">Factory for monitor loggers</param>
    /// <param name="logDataFactory">Current log data factory</param>
    /// <param name="loggerId">Logger ID of the device</param>
    /// <param name="appLoggerFactory">Current logger proxy factory</param>
    /// <returns>Monitor logger instance</returns>
    protected static IAppLoggerProxy CreateMonitorLogger(IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory, IAppLoggerProxyFactory appLoggerFactory, ILogDataFactory logDataFactory, string loggerId)
    {
        var monitorLoggerFactory = monitorLoggerFactoryFactory.CreateInstance(loggerId);
        var logger = appLoggerFactory.CreateInstance(monitorLoggerFactory, logDataFactory);
        return logger;
    }

    #endregion

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
    /// <param name="loggerId">Logger ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    public virtual void CreateMessagingConfig(string loggerId, string ipAddress, int port)
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
    /// Configure the order management
    /// </summary>
    /// <param name="orderManagerFactory">Current factory for <see cref="IOrderManager"/> instances</param>
    public virtual void ConfigureOrderManagement(IOrderManagerFactory orderManagerFactory)
    {
        // Do nothing
    }

    /// <summary>
    /// Configure the order management and if necessary the state management. Important: store state factory instance to device and config
    /// </summary>
    /// <param name="deviceStateManagerFactory">Current factory for <see cref="IDeviceStateManager"/> instances</param>
    /// <param name="stateMachineConfiguratorFactory">Current state machine configurator factory</param>
    public virtual void ConfigureStateManagement(IDeviceStateManagerFactory deviceStateManagerFactory, 
        IStateMachineConfiguratorFactory stateMachineConfiguratorFactory)
    {
        // Do nothing
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