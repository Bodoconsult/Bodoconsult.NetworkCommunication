// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Default device states
/// </summary>
public static class DefaultDeviceStates
{
    /// <summary>
    /// device firmware is not reachable via network. Hardware code: 0x07
    /// </summary>
    public static DeviceState DeviceStateOffline = new(0, "Device is not reachable via network");


    /// <summary>
    /// Device is reachable via network. Hardware code: not existing 
    /// </summary>
    public static DeviceState DeviceStateOnline = new(1, "Device is reachable via network");

    /// <summary>
    /// Device is ready for order processing
    /// </summary>
    public static DeviceState DeviceStateReady = new(3, "Device is ready for order processing");

}