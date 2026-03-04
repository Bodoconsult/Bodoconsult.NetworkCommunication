// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.TestData;

/// <summary>
/// Device hardware states
/// </summary>
public static class TestDeviceStates
{
    /// <summary>
    /// device firmware is not reachable via network. Hardware code: 0x07
    /// </summary>
    public static IDeviceState DeviceStateOffline =
        new DeviceState(0, "Device is not reachable via network");


    /// <summary>
    /// Device is reachable via network. Hardware code: not existing 
    /// </summary>
    public static IDeviceState DeviceStateOnline =
        new DeviceState(1, "Device is reachable via network");

    /// <summary>
    /// Device is ready for order processing
    /// </summary>
    public static IDeviceState DeviceStateReady =
        new DeviceState(3, "Device is ready for order processing");
}