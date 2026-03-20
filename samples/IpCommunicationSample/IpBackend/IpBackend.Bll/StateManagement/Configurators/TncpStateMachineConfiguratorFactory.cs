// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.StateManagement.Configurators;

/// <summary>
/// Factory for creating a <see cref="TncpStateMachineConfigurator "/> instance
/// </summary>
public class TncpStateMachineConfiguratorFactory : IStateMachineConfiguratorFactory
{
    /// <summary>
    /// Create a configurator for a certain device
    /// </summary>
    /// <param name="deviceManagerState">Current device manager state</param>
    /// <returns></returns>
    public IStateMachineConfigurator CreateInstance(IStateMachineDeviceBusinessLogicAdapter deviceManagerState)
    {
        return new TncpStateMachineConfigurator(deviceManagerState);
    }
}