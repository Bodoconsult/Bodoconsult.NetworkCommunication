// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Benchmarking;
using Bodoconsult.NetworkCommunication.App.Abstractions;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Notification data for a [state machine state changed] event 
/// </summary>
public class StateMachineStateNotification : BaseClientNotification
{
    /// <summary>
    /// Current device state ID
    /// </summary>
    public int DeviceStateId { get; set; }

    /// <summary>
    /// Current device state name or null
    /// </summary>
    public string? DeviceStateName { get; set; }

    /// <summary>
    /// Current business state ID
    /// </summary>
    public int BusinessStateId { get; set; }

    /// <summary>
    /// Current business state name or null
    /// </summary>
    public string? BusinessStateName { get; set; }

    /// <summary>
    /// Current business substate ID
    /// </summary>
    public int BusinessSubstateId { get; set; }

    /// <summary>
    /// Current business substate name or null
    /// </summary>
    public string? BusinessSubstateName { get; set; }
}