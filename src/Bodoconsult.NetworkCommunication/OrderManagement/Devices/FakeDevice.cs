// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// A fake device supporting order management
/// </summary>
public class FakeDevice : BaseOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public FakeDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : base(dataMessagingConfig, clientNotificationManager)
    { }

    /// <summary>
    /// Load a comm adapter
    /// </summary>
    /// <param name="commAdapter">Current comm adapter</param>
    public void LoadCommAdapter(ICommunicationAdapter commAdapter)
    {
        CommunicationAdapter = commAdapter;
    }
}