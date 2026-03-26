// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a device with state management and order management
/// </summary>
public interface IStateMachineDeviceFactory
{
    /// <summary>
    /// Create a device for order management and state management
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    /// <param name="deviceStateCheckManager">Curent state checker implementation</param>
    IStateMachineDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig, IDeviceStateCheckManager deviceStateCheckManager);
}