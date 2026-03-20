// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="TncpBackendTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class TncpBackendTcpIpDeviceBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        if (device is not IStateMachineDevice statemachineDevice)
        {
            throw new ArgumentException($"device is not implementing {nameof(IStateMachineDevice)}");
        }

        return new TncpBackendTcpIpBusinessLogicAdapter(statemachineDevice);
    }
}