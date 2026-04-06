// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;

namespace IpClient.Bll.Delegates;

/// <summary>
/// Delegate fired when then state of the backend has changed
/// </summary>
/// <param name="reqestData">Current request data</param>
public delegate void StateChangedNotificationDelegate(StateChangedEventFiredBusinessTransactionRequestData reqestData);