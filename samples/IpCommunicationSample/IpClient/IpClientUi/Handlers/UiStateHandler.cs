// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpClient.Bll.Interfaces;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Text;
using IpClient.Bll.Delegates;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

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
        LastDeviceError = string.Empty;
    }

    /// <summary>
    /// Current state changed fired request data or null
    /// </summary>
    public StateChangedEventFiredBusinessTransactionRequestData? CurrentRequestData { get; private set; }


    /// <summary>
    /// Current request data for the device reported an error to the backend
    /// </summary>
    public ErrorBusinessTransactionRequestData? ErrorBusinessTransactionRequestData { get; private set; }

    /// <summary>
    /// Device state message to show in the UI
    /// </summary>
    [Reactive] public partial string DeviceStateMessage { get; set; }

    /// <summary>
    /// Backend state message to show in the UI
    /// </summary>
    [Reactive] public partial string BackendStateMessage { get; set; }

    /// <summary>
    /// Counter for device errors
    /// </summary>
    [Reactive] public partial int DeviceErrorCounter { get; set; }

    /// <summary>
    /// Last device error reported
    /// </summary>
    [Reactive] public partial string LastDeviceError { get; set; }

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

    public void ReportDeviceErrorReceived(ErrorBusinessTransactionRequestData requestData)
    {
        ErrorBusinessTransactionRequestData = requestData;

        if (DeviceErrorCounter == int.MaxValue)
        {
            DeviceErrorCounter = 0;
        }

        DeviceErrorCounter++;
        LastDeviceError = $"Command: {requestData.TelnetCommand}\r\nAdditional info:\r\n{requestData.TelnetAdditionalInfo}";
    }
}
