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
    IIpDataMessagingConfig? DataMessagingConfig { get; }

    /// <summary>
    /// Current device
    /// </summary>
    IIpDevice? Device { get; }

    /// <summary>
    /// Create the basic data messaging config
    /// </summary>
    /// <param name="loggerId">Logger ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="port">Port</param>
    /// <param name="messageProcessingPackageFactory">Current data messaging package factory</param>
    void CreateMessagingConfig(string loggerId, string ipAddress, int port, IDataMessageProcessingPackageFactory messageProcessingPackageFactory);

    /// <summary>
    /// Create the device with basic settings
    /// </summary>
    /// <param name="businessLogicAdapterFactory">Current factory for <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instances</param>
    void CreateDevice(IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory);

    /// <summary>
    /// Configure the order management
    /// </summary>
    /// <param name="orderManagerFactory">Current factory for <see cref="IOrderManager"/> instances</param>
    void ConfigureOrderManagement(IOrderManagerFactory orderManagerFactory);

    /// <summary>
    /// Configure the state management. Important: store state factory instance to device and config
    /// </summary>
    /// <param name="stateMachineConfiguratorFactory">Current state machine configurator factory</param>
    void ConfigureStateManagement(IStateMachineConfiguratorFactory stateMachineConfiguratorFactory);

    /// <summary>
    /// Get the read configured device
    /// </summary>
    /// <returns>Freshly created and configured <see cref="IIpDevice"/> instance</returns>
    IIpDevice GetDevice();
}