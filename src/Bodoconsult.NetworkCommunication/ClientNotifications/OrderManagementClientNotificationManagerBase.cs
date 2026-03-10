// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Current implementation of <see cref="IOrderManagementClientNotificationManager"/> for central event notification handling
/// </summary>
public abstract class OrderManagementClientNotificationManagerBase : IOrderManagementClientNotificationManager
{
    #region Delegate definitions

    /// <summary>
    /// Delegate for sending a notification to the client
    /// </summary>
    public TransferToClientDelegate NotifyClient { get; set; }

    #endregion

    public void DoNotifyStateManagementStateEvent(object sender, IStateMachineState state)
    {
        var notification = new StateMachineStateNotification
        {
            State = state
        };

        NotifyClient?.Invoke(sender, notification);
    }

    /// <summary>
    /// Send a progress notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="currentProgressType">Current progress type. Define your own types in an enum</param>
    /// <param name="percentage">Current percentage</param>
    /// <param name="complete">Is completed?</param>
    public void DoNotifyProgressEvent(object sender, int currentProgressType, int percentage, bool complete)
    {
        var notification = new ProgressNotification
        {
            Completed = complete,
            Type = currentProgressType,
            Progress = percentage
        };

        NotifyClient?.Invoke(sender, notification);
    }

    /// <summary>
    /// Send an exception notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Exception to report</param>
    public void DoNotifyException(object sender, Exception e)
    {
        var notification = new ExceptionNotification
        {
            Exception = e
        };

        NotifyClient?.Invoke(sender, notification);
    }

    /// <summary>
    /// Do notify order state changed
    /// </summary>
    /// <param name="sender">Sender of the notification</param>
    /// <param name="order">Current order</param>
    public void DoNotifyOrderStateChanged(object sender, IOrder order)
    {
        var notification = new OrderExecutionNotification
        {
            Order = order,
        };

        NotifyClient?.Invoke(sender, notification);
    }
}