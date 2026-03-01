// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// A simple device supporting order management
/// </summary>
public class SimpleDevice: BaseOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    public SimpleDevice(IDataMessagingConfig dataMessagingConfig) : base(dataMessagingConfig)
    {
    }
}