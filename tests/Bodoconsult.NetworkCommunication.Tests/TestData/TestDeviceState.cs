// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.TestData;

/// <summary>
/// Device hardware states
/// </summary>
public class TestDeviceState : IDeviceState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public TestDeviceState(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// The ID of the state
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    public string Name { get; set; }


    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Id} {Name}";

    /// <summary>
    /// Tower firmware is not reachable via network. Hardware code: 0x07
    /// </summary>
    public static TestDeviceState DeviceStateOffline =
        new TestDeviceState(0, "Device is not reachable via network");


    /// <summary>
    /// Device is reachable via network. Hardware code: not existing 
    /// </summary>
    public static TestDeviceState DeviceStateOnline =
        new TestDeviceState(1, "Device is reachable via network");

    /// <summary>
    /// Device is ready for order processing
    /// </summary>
    public static TestDeviceState DeviceStateReady =
        new TestDeviceState(3, "Device is ready for order processing");
}