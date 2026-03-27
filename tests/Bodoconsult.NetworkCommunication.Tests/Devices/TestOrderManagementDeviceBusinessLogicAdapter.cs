// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Devices;

internal class TestOrderManagementDeviceBusinessLogicAdapter : BaseOrderManagementDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TestOrderManagementDeviceBusinessLogicAdapter(IOrderManagementDevice device) : base(device)
    { }
}