// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Notification data for a [state management state changed] event 
/// </summary>
public class StateManagementStateNotification : BaseClientNotification
{
    /// <summary>
    /// The current state to report
    /// </summary>
    public IStateManagementState State { get; set; }
}