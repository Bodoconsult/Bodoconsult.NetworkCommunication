// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Device.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSample.Device.Bll.BusinessLogic.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="TncpBackendTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class TncpBackendTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public TncpBackendTcpIpBusinessLogicAdapterFactory(IBusinessTransactionManager businessTransactionManager)
    {
        _businessTransactionManager = businessTransactionManager;
    }
    /// <summary>
    /// Create an instance of <see cref="TncpBackendTcpIpBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device)
    {
        return new TncpBackendTcpIpBusinessLogicAdapter(device, _businessTransactionManager);
    }
}