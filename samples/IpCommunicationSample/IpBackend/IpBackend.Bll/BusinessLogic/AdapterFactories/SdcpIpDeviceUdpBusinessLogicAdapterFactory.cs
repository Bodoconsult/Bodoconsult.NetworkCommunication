// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.Adapters;

namespace IpBackend.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="SfxpIpDeviceUdpBusinessLogicAdapter"/> instances
/// </summary>
public class SfxpIpDeviceUdpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public SfxpIpDeviceUdpBusinessLogicAdapterFactory(IBusinessTransactionManager businessTransactionManager)
    {
        _businessTransactionManager = businessTransactionManager;
    }

    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        return new SfxpIpDeviceUdpBusinessLogicAdapter(device, _businessTransactionManager);
    }
}