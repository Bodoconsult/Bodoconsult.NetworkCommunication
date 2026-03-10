// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Configuration for a <see cref="INoActionStateMachineState"/>. State machine state configurations have to set per device
/// </summary>
public interface INoActionStateConfiguration : IStateConfiguration
{
    /// <summary>
    /// Delegate to be executed from a no action state machine state to check if job states are required to be executed
    /// </summary>
    CheckJobstatesActionForStateDelegate CheckJobstatesActionForStateDelegate { get; set; }
}