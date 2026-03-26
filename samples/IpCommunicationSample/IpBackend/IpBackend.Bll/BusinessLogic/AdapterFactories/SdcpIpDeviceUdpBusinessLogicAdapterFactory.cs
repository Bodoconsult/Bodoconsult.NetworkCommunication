// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="SdcpIpDeviceUdpBusinessLogicAdapter"/> instances
/// </summary>
public class SdcpIpDeviceUdpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        return new SdcpIpDeviceUdpBusinessLogicAdapter(device);
    }
}