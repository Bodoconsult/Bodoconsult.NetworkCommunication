// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Types of state machine states
/// </summary>
public enum StateMachineStateTypes
{
    /// <summary>
    /// State running order management order as action
    /// </summary>
    OrderBasedActionState,
    /// <summary>
    /// State running a code delegate as action
    /// </summary>
    OrderlessActionState,
    /// <summary>
    /// State running no action (simply wait)
    /// </summary>
    NoActionState,
    /// <summary>
    /// State running actions bound to a job
    /// </summary>
    JobActionState
}