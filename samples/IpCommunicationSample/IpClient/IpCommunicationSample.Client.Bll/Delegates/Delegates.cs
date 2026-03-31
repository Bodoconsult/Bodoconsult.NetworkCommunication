// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Client.Bll.Delegates;

/// <summary>
/// Delegate fired when then state of the backend has changed
/// </summary>
/// <param name="reqestData">Current request data</param>
public delegate void StateChangedNotificationDelegate(StateChangedEventFiredBusinessTransactionRequestData reqestData);