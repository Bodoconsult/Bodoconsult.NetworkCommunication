// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// A delegate use to transfer data to the client
/// </summary>
/// <param name="source">Source object of current notification</param>
/// <param name="notification">Current notification</param>
public delegate void TransferToClientDelegate(object source, IClientNotification notification);

/// <summary>
/// Interface for central event message handling for client notifications
/// </summary>
public interface ICentralClientNotificationManager 
{
    #region Delegate definitions

    /// <summary>
    /// Delegate for sending a notification to the client
    /// </summary>
    TransferToClientDelegate? NotifyClient { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Send a progress notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="currentProgressType">Current progress type. Define your own types in an enum</param>
    /// <param name="percentage">Current percentage</param>
    /// <param name="complete">Is completed?</param>
    void DoNotifyProgressEvent(object sender, int currentProgressType, int percentage, bool complete);

    /// <summary>
    /// Send an exception notification
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Exception to report</param>
    void DoNotifyException(object sender, Exception e);

    #endregion
}