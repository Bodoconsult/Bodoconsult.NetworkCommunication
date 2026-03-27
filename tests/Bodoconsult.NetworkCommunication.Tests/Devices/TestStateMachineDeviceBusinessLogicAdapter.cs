// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Devices;

internal class TestStateMachineDeviceBusinessLogicAdapter : BaseStateMachineDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TestStateMachineDeviceBusinessLogicAdapter(IStateMachineDevice device) : base(device)
    { }
}