// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Factory interface for creating instances of <see cref="IStateMachineConfigurator"/> to configure a device
/// </summary>
public interface IStateMachineConfiguratorFactory
{
    /// <summary>
    /// Create a configurator for a certain device
    /// </summary>
    /// <param name="deviceManagerState">Current device manager state</param>
    /// <returns></returns>
    IStateMachineConfigurator CreateInstance(IDeviceStateManager deviceManagerState);
}