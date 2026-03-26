// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for state machine configurators configuring a <see cref="IStateMachineStateFactory"/> instance for a state machine
/// </summary>
public interface IStateMachineConfigurator
{
    /// <summary>
    /// Current manager for state management devices
    /// </summary>
    IStateMachineDeviceBusinessLogicAdapter DeviceBusinessLogicAdapter{ get; }

    /// <summary>
    /// Configure the factory as required
    /// </summary>
    void ConfigureFactory();

    /// <summary>
    /// Configure the factory for a state machine
    /// </summary>
    /// <returns><see cref="IStateMachineStateFactory"/> instance for a state machine</returns>
    IStateMachineStateFactory BuildFactory();
}