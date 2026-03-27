// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Client.Bll.Interfaces;

namespace IpCommunicationSample.Client.Bll.BusinessTransactions.Adapters;

/// <summary>
/// Current adapter for TCP/IP channel from client to backend
/// </summary>
public class BtcpBackendTcpIpBusinessLogicAdapter : BaseOrderManagementDeviceBusinessLogicAdapter, IBackendTcpIpBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device supporting order management</param>
    public BtcpBackendTcpIpBusinessLogicAdapter(IOrderManagementDevice device) : base(device)
    { }

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStartStreamingState(IBusinessTransactionRequestData request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Request a start snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStopStreamingState(IBusinessTransactionRequestData request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    public void RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request)
    {
        throw new NotImplementedException();
    }
}