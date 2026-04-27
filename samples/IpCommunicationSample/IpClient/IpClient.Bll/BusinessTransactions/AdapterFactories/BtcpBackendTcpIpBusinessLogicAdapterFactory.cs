// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using IpClient.Bll.BusinessTransactions.Adapters;
using IpClient.Bll.Interfaces;

namespace IpClient.Bll.BusinessTransactions.AdapterFactories;

/// <summary>
/// Current implementation of <see cref="IDeviceBusinessLogicAdapterFactory"/> delivering <see cref="BtcpBackendTcpIpBusinessLogicAdapter"/> instances
/// </summary>
public class BtcpBackendTcpIpBusinessLogicAdapterFactory : IDeviceBusinessLogicAdapterFactory
{
    private readonly IOrderIdGenerator _orderIdGenerator;
    private readonly IUiStateHandler _uiStateHandler;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="orderIdGenerator">Current order ID generator</param>
    /// <param name="uiStateHandler">Current UI state handler</param>
    public BtcpBackendTcpIpBusinessLogicAdapterFactory(IOrderIdGenerator orderIdGenerator, IUiStateHandler uiStateHandler)
    {
        _orderIdGenerator = orderIdGenerator;
        _uiStateHandler = uiStateHandler;
    }

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

        return new BtcpBackendTcpIpBusinessLogicAdapter(orderManagementDevice, _orderIdGenerator, _uiStateHandler);
    }
}