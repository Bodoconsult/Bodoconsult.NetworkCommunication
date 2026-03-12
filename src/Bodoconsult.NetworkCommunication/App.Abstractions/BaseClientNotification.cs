// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Newtonsoft.Json;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Base class for client notifications based on <see cref="IClientNotification"/>
/// </summary>
public abstract class BaseClientNotification : IClientNotification
{
    /// <summary>
    /// An ID set by the client manager to identify the client notification
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The notification object to send via transport layer to the client
    /// </summary>
    [JsonIgnore]
    public object? NotificationObjectToSend { get; set; }

    /// <summary>
    /// Add to queue if the conenction to the client has currently failed
    /// </summary>
    public bool AddToQueueOnBrokenConnection { get; set; }

    ///// <summary>
    ///// The client IDs of all clients the sending of the notification is required
    ///// </summary>
    //public IList<string> ClientIds { get; } = new List<string>();

    /// <summary>
    /// The client IDs of all clients the sending of the notification failed
    /// </summary>
    public IList<string> ClientIdsNotificationFailed { get; set; } = new List<string>();
}