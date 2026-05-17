// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Basic implementation of <see cref="IClientMessagingService"/>
/// </summary>
public class BasicBtcpNetworkingClientMessagingService : BaseBtcpNetworkingClientMessagingService
{
    /// <summary>
    /// Default ctor loading <see cref="StateMachineStateNotification"/> notifications
    /// </summary>
    public BasicBtcpNetworkingClientMessagingService(IAppGlobals appGlobals) : base(appGlobals)
    { }
}