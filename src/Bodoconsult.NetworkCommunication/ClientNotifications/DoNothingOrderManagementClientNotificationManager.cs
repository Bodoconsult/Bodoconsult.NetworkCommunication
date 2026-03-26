// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Fake implementation of <see cref="IOrderManagementClientNotificationManager"/> doing nothing
/// </summary>
public class DoNothingOrderManagementClientNotificationManager : IOrderManagementClientNotificationManager
{
    /// <summary>
    /// Delegate for sending a notification to the client
    /// </summary>
    public TransferToClientDelegate? NotifyClient { get; set; }

    /// <summary>
    /// Send a progress notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="currentProgressType">Current progress type. Define your own types in an enum</param>
    /// <param name="percentage">Current percentage</param>
    /// <param name="complete">Is completed?</param>
    public void DoNotifyProgressEvent(object sender, int currentProgressType, int percentage, bool complete)
    {
        // Do nothing
    }

    /// <summary>
    /// Send an exception notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Exception to report</param>
    public void DoNotifyException(object sender, Exception e)
    {
        // Do nothing
    }

    /// <summary>
    /// Send a string notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="state">State to send to client</param>
    public void DoNotifyStateManagementStateEvent(object sender, IStateMachineState state)
    {
        // Do nothing
    }

    /// <summary>
    /// Do notify order state changed
    /// </summary>
    /// <param name="sender">Sender of the notification</param>
    /// <param name="order">Current order</param>
    public void DoNotifyOrderStateChanged(object sender, IOrder order)
    {
        // Do nothing
    }
}