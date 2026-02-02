// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface to implemented order management related messaging to clients etc.
/// </summary>
public interface IOrderManagementMessaging
{
    /// <summary>
    /// Do notify order state changed
    /// </summary>
    /// <param name="sender">Sender of the notification</param>
    /// <param name="order">Current order</param>
    void DoNotifyOrderStateChanged(object sender, IOrder order);
}