// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Base interface for state machine state builder configurations. State machine state configurations have to set per device
/// </summary>
public interface IStateConfiguration
{
    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementDevice CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    IStateMachineStateBuilder StateBuilderBuilder { get; set; }

    /// <summary>
    /// Delegate to handle a ComDevClose event in business logic
    /// </summary>
    HandleComDevCloseDelegate HandleComDevCloseDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    HandleErrorMessageDelegate HandleErrorMessageDelegate { get; set; }

    /// <summary>
    /// Handle an async received message
    /// </summary>
    HandleAsyncMessageDelegate HandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Delegate for preparing orders for the regular state reqeust
    /// </summary>
    PrepareRegularStateRequestDelegate PrepareRegularStateRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling device state check request answers in business logic
    /// </summary>
    HandleRegularStateRequestAnswerDelegate HandleRegularStateRequestAnswerDelegate { get; set; }
}