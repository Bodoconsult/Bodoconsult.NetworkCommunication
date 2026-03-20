// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="TncpBackendBusinessLogicAdapter"/> instances
/// </summary>
public class TncpBackendDeviceBusinessLogicAdapterFactory: IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IStateMachineDeviceBusinessLogicAdapter CreateInstance(IStateManagementDevice device)
    {
        return new TncpBackendBusinessLogicAdapter(device);
    }
}