// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Factories;

namespace Bodoconsult.NetworkCommunication.StateManagement.Configurators;

/// <summary>
/// Base class for <see cref="IStateMachineConfigurator"/> instances
/// </summary>
public abstract class BaseStateMachineConfigurator: IStateMachineConfigurator
{
    /// <summary>
    /// Current <see cref="IStateMachineStateFactory"/> instance to configure
    /// </summary>
    protected IStateMachineStateFactory StateFactory = new StateMachineStateFactory();

    /// <summary>
    /// Current manager for state management devices
    /// </summary>
    public IStateMachineDeviceBusinessLogicAdapter DeviceBusinessLogicAdapter{ get; }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessLogicAdapter">Current device state manager</param>
    protected BaseStateMachineConfigurator(IStateMachineDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        DeviceBusinessLogicAdapter= businessLogicAdapter;
    }

    /// <summary>
    /// Configure the factory as required
    /// </summary>
    public virtual void ConfigureFactory()
    {
        throw new NotSupportedException("Override in derived class!");
    }

    /// <summary>
    /// Configure the factory for a state machine
    /// </summary>
    /// <returns><see cref="IStateMachineStateFactory"/> instance for a state machine</returns>
    public IStateMachineStateFactory BuildFactory()
    {
        return StateFactory;
    }
}