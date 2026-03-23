// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Client.Bll.Interfaces;

/// <summary>
/// Interface for the buisness logic adapter for the TCP/IP channel to the client
/// </summary>
public interface IBackendTcpIpBusinessLogicAdapter : IOrderManagementDeviceBusinessLogicAdapter
{
    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    void RequestDeviceStartStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a start snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    void RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    void RequestDeviceStopStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    void RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request);

    #endregion


}
