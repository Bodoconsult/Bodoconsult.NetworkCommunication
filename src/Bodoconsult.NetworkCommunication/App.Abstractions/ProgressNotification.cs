// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Notification data for [progress changed] event
/// </summary>
public class ProgressNotification : BaseClientNotification
{
    /// <summary>
    /// Type of progress event
    /// </summary>
    public int Type;

    /// <summary>
    /// Current progress value
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// Has completed?
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// Current value
    /// </summary>
    public int ActualValue { get; set; }

    /// <summary>
    /// Maximum value
    /// </summary>
    public int MaxValue { get; set; }
}