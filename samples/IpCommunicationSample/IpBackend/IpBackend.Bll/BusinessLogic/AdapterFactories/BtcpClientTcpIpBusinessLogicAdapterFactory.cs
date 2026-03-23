// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="BtcpClientTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class BtcpClientTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="ISimpleDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        return new BtcpClientTcpIpBusinessLogicAdapter(device);
    }
}