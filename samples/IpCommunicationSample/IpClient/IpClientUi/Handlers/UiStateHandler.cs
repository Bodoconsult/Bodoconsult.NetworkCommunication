// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.ReactiveUI.ViewModels;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpClient.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.Handlers;

/// <summary>
/// Current implementation of <see cref="IUiStateHandler"/>
/// </summary>
public partial class UiStateHandler : ReactiveObject, IUiStateHandler
{
    private readonly CopyrightViewModel _copyrightViewModel;

    /// <summary>
    /// Counter for device errors
    /// </summary>
    private int _deviceErrorCounter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public UiStateHandler(CopyrightViewModel copyrightViewModel)
    {
        DeviceStateMessage = "App is loading...";
        BackendStateMessage = DeviceStateMessage;
        LastDeviceError = string.Empty;
        _copyrightViewModel = copyrightViewModel;
        DeviceErrorCounterString = "0";
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
    /// Current FFT request data
    /// </summary>
    public FftReportBusinessTransactionRequestData? FftReportBusinessTransactionRequestData { get; private set; }

    /// <summary>
    /// Device state message to show in the UI
    /// </summary>
    [Reactive] public partial string DeviceStateMessage { get; set; }

    /// <summary>
    /// Backend state message to show in the UI
    /// </summary>
    [Reactive] public partial string BackendStateMessage { get; set; }

    /// <summary>
    /// Device error counter as string
    /// </summary>
    [Reactive] public partial string DeviceErrorCounterString { get; set; }

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

        if (requestData.ModuleInfo == null)
        {
            return;
        }

        _copyrightViewModel.LoadModule(requestData.ModuleInfo);
    }

    public void ReportDeviceErrorReceived(ErrorBusinessTransactionRequestData requestData)
    {
        ErrorBusinessTransactionRequestData = requestData;

        if (_deviceErrorCounter == int.MaxValue)
        {
            _deviceErrorCounter = 0;
        }

        _deviceErrorCounter++;
        DeviceErrorCounterString = _deviceErrorCounter.ToString("0,000");
        LastDeviceError = $"Command: {requestData.TelnetCommand}\r\nAdditional info:\r\n{requestData.TelnetAdditionalInfo}";
    }

    /// <summary>
    /// An FFT request was received from backend
    /// </summary>
    /// <param name="requestData">Current FFT request data</param>
    public void FftReportBusinessTransactionRequestDataReceived(FftReportBusinessTransactionRequestData requestData)
    {
        FftReportBusinessTransactionRequestData = requestData;
    }
}
