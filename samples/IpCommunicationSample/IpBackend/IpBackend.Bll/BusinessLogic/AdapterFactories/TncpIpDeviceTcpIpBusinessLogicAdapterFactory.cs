// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.Adapters;

namespace IpBackend.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="TncpIpDeviceTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class TncpIpDeviceTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public TncpIpDeviceTcpIpBusinessLogicAdapterFactory(IBusinessTransactionManager businessTransactionManager)
    {
        _businessTransactionManager = businessTransactionManager;
    }

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

        return new TncpIpDeviceTcpIpBusinessLogicAdapter(statemachineDevice, _businessTransactionManager);
    }
}