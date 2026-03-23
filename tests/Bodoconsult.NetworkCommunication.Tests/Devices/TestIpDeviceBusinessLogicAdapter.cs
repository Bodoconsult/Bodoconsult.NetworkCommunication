// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.Tests.Devices;

internal class TestIpDeviceBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TestIpDeviceBusinessLogicAdapter(IIpDevice device) : base(device)
    { }
}