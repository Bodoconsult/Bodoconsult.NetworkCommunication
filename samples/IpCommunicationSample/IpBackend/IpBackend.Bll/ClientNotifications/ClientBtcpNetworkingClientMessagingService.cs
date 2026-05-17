// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.ClientNotifications;

namespace IpBackend.Bll.ClientNotifications;

public class ClientBtcpNetworkingClientMessagingService : BaseBtcpNetworkingClientMessagingService
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public ClientBtcpNetworkingClientMessagingService(IAppGlobals appGlobals) : base(appGlobals)
    { }
}