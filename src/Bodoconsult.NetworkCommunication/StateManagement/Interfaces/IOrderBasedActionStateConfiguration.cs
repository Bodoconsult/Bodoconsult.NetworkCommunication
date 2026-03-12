// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Configuration for a <see cref="IOrderBasedActionStateMachineState"/>. State machine state configurations have to set per device
/// </summary>
public interface IOrderBasedActionStateConfiguration: IStateConfiguration
{
    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedSucessfullyDelegate? OrderFinishedSucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedUnsucessfullyDelegate? OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate to create one or more orders sent to device needed for an order based state machine state
    /// </summary>
    PrepareOrdersForStateMachineStateDelegate? PrepareOrdersForStateMachineStateDelegate { get; set; }
}