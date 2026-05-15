// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpClient.Bll.Delegates;

/// <summary>
/// Delegate fired when then state of the backend has changed
/// </summary>
/// <param name="requestData">Current request data</param>
public delegate void StateChangedNotificationDelegate(StateChangedEventFiredBusinessTransactionRequestData requestData);

/// <summary>
/// Delegate fired when the device reported an error to the backend
/// </summary>
/// <param name="requestData">Current request data</param>
public delegate void ReportDeviceErrorDelegate(ErrorBusinessTransactionRequestData requestData);