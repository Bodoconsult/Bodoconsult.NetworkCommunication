// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpClient.Bll.Delegates;

namespace IpClient.Bll.Interfaces;

/// <summary>
/// Interface for the buisness logic adapter for the TCP/IP channel to the client
/// </summary>
public interface IBackendTcpIpBusinessLogicAdapter : IOrderManagementDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Delegate fired when then state of the backend has changed
    /// </summary>
    StateChangedNotificationDelegate? StateChangedNotificationDelegate { get; }

    #region State management

    /// <summary>
    /// Request a start messaging state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStartMessagingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop messaging state
    /// </summary>
    /// <param name="request">Current request</param>
    IBusinessTransactionReply RequestDeviceStopMessagingState(IBusinessTransactionRequestData request);


    #endregion


    #region Notifications


    /// <summary>
    /// Notification fired
    /// </summary>
    /// <param name="requestData">Current request data</param>
    /// <returns>Returns <see cref="DoNotSendBusinessTransactionReply"/></returns>
    IBusinessTransactionReply NotificationFired(IBusinessTransactionRequestData requestData);

    #endregion

    #region Reporting

    /// <summary>
    /// Create an FFT analysis report
    /// </summary>
    /// <param name="requestData"></param>
    /// <returns></returns>
    IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData);

    #endregion


}
