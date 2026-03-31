// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions.BusinessTransactions;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Client.Bll.Delegates;

namespace IpCommunicationSample.Client.Bll.Interfaces;

/// <summary>
/// Interface for the buisness logic adapter for the TCP/IP channel to the client
/// </summary>
public interface IBackendTcpIpBusinessLogicAdapter : IOrderManagementDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Delegate fired when then state of the backend has changed
    /// </summary>
    StateChangedNotificationDelegate? StateChangedNotificationDelegate { get; set; }

    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStartStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a start snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStopStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request);


    #endregion

    /// <summary>
    /// Notification fired
    /// </summary>
    /// <param name="requestData">Current request data</param>
    /// <returns>Returns <see cref="DoNotSendBusinessTransactionReply"/></returns>
    IBusinessTransactionReply NotificationFired(IBusinessTransactionRequestData requestData);
}
