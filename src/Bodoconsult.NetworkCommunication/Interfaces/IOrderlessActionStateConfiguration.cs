// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Configuration for a <see cref="IOrderlessActionStateMachineState"/>. State machine state configurations have to set per device
/// </summary>
public interface IOrderlessActionStateConfiguration : IStateConfiguration
{
    /// <summary>
    /// Delegate to be executed from an orderless state machine state
    /// </summary>
    ExecuteActionForStateDelegate? ExecuteActionForStateDelegate { get; set; }
}