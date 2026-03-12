// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Notification data for a [state machine state changed] event 
/// </summary>
public class StateMachineStateNotification : BaseClientNotification
{
    /// <summary>
    /// The current state to report
    /// </summary>
    public IStateMachineState? State { get; set; }
}