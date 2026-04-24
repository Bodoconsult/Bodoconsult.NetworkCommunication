// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Basic implementation of <see cref="IOrderManagementClientNotificationManager"/>
/// </summary>
public class BasicClientNotificationManager : OrderManagementClientNotificationManagerBase
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="clientMessagingBusinessDelegate">Client messaging business delegate</param>
    public BasicClientNotificationManager(IClientMessagingBusinessDelegate clientMessagingBusinessDelegate)
    {
        NotifyClient = clientMessagingBusinessDelegate.MessagingBusinessManagerOnNotifyClient;
    }
}