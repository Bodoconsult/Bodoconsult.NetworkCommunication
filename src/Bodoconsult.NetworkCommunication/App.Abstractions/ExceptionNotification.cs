// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Notification data for [exception has been thrown] event
/// </summary>
public class ExceptionNotification : BaseClientNotification
{
    /// <summary>
    /// Exception to report
    /// </summary>
    public Exception? Exception { get; set; }
}