// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface to implemented order management related messaging to clients etc.
/// </summary>
public interface IOrderManagementClientNotificationManager : ICentralClientNotificationManager
{
    /// <summary>
    /// Send a string notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="state">State to send to client</param>
    void DoNotifyStateManagementStateEvent(object sender, IStateManagementState state);

    /// <summary>
    /// Do notify order state changed
    /// </summary>
    /// <param name="sender">Sender of the notification</param>
    /// <param name="order">Current order</param>
    void DoNotifyOrderStateChanged(object sender, IOrder order);
}