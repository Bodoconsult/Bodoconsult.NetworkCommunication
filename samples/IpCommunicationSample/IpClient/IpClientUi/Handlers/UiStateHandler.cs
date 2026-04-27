// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpClient.Bll.Interfaces;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpClientUi.Handlers;

/// <summary>
/// Current implementation of <see cref="IUiStateHandler"/>
/// </summary>
public partial class UiStateHandler : ReactiveObject, IUiStateHandler
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public UiStateHandler()
    {
        DeviceStateMessage = "App is loading...";
        BackendStateMessage = DeviceStateMessage;
    }

    /// <summary>
    /// Current state changed fired request data or null
    /// </summary>
    public StateChangedEventFiredBusinessTransactionRequestData? CurrentRequestData { get; private set; }

    /// <summary>
    /// Device state message to show in the UI
    /// </summary>
    [Reactive] public partial string DeviceStateMessage { get; set; }

    /// <summary>
    /// Backend state message to show in the UI
    /// </summary>
    [Reactive] public partial string BackendStateMessage { get; set; }

    /// <summary>
    /// Method to receive a state changed event fired request
    /// </summary>
    /// <param name="requestData">Current request data</param>
    public void StateChangedNotificationReceived(StateChangedEventFiredBusinessTransactionRequestData requestData)
    {
        DeviceStateMessage = string.Empty;
        BackendStateMessage = string.Empty;

        CurrentRequestData = requestData;

        var msg = new StringBuilder();

        msg.Append($"Business state: {requestData.BusinessStateId} {requestData.BusinessStateName}");

        if (requestData.BusinessSubstateId > 0)
        {
            msg.Append($"   ({requestData.BusinessSubstateId} {requestData.BusinessSubstateName})");
        }

        DeviceStateMessage = $"Device state:  {requestData.DeviceStateId} {requestData.DeviceStateName}";
        BackendStateMessage = msg.ToString();
    }
}
