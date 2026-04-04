// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.ClientNotifications;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;

/// <summary>
/// Notification data for an [order execution state has changed] event 
/// </summary>
/// <remarks>Interface is required for mocking with MOQ</remarks>
public class OrderExecutionNotification : BaseClientNotification, IOrderExecutionNotification
{
    /// <summary>
    /// The current order
    /// </summary>
    public IOrder? Order { get; set; }
}