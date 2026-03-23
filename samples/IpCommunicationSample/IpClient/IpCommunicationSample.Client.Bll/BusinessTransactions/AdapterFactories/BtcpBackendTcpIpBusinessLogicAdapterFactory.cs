// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using IpCommunicationSample.Client.Bll.BusinessTransactions.Adapters;

namespace IpCommunicationSample.Client.Bll.BusinessTransactions.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="BtcpBackendTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class BtcpBackendTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        if (device is not IOrderManagementDevice orderManagementDevice)
        {
            throw new ArgumentException($"device is not implementing {nameof(IOrderManagementDevice)}");
        }

        return new BtcpBackendTcpIpBusinessLogicAdapter(orderManagementDevice);
    }
}