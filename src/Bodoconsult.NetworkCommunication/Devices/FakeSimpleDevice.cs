// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// A fake device supporting state management and order management
/// </summary>
public class FakeSimpleDevice : BaseSimpleDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public FakeSimpleDevice(IDataMessagingConfig dataMessagingConfig, ICentralClientNotificationManager clientNotificationManager) : 
        base(dataMessagingConfig, clientNotificationManager)
    {
    }
}