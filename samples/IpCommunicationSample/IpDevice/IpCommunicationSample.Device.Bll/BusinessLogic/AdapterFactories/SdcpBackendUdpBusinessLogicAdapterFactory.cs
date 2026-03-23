// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Device.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSample.Device.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="SdcpBackendUdpBusinessLogicAdapter"/> instances
/// </summary>
public class SdcpBackendUdpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="SdcpBackendUdpBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        return new SdcpBackendUdpBusinessLogicAdapter(device);
    }
}