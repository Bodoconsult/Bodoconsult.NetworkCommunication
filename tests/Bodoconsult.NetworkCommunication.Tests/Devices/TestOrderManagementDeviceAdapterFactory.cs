// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Devices;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="TestIpDeviceConfigurator"/> instances
/// </summary>
public class TestOrderManagementDeviceAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="TestOrderManagementDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        if (device is not IOrderManagementDevice omDevice)
        {
            throw new ArgumentException($"device is not implementing {nameof(IOrderManagementDevice)}");
        }

        return new TestOrderManagementDeviceBusinessLogicAdapter(omDevice);
    }
}