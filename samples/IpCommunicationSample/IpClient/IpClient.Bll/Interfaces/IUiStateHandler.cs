// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using IpClient.Bll.Delegates;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpClient.Bll.Interfaces;

/// <summary>
/// Interface for UI state message handling
/// </summary>
public interface IUiStateHandler
{
    /// <summary>
    /// Current state changed fired request data or null
    /// </summary>
    StateChangedEventFiredBusinessTransactionRequestData? CurrentRequestData { get; }

    /// <summary>
    /// Current request data for the device reported an error to the backend
    /// </summary>
    ErrorBusinessTransactionRequestData? ErrorBusinessTransactionRequestData { get; }

    /// <summary>
    /// Device state message to show in the UI
    /// </summary>
    string DeviceStateMessage { get; set; }

    /// <summary>
    /// Backend state message to show in the UI
    /// </summary>
    string BackendStateMessage { get; set; }


    int DeviceErrorCounter { get; set; }

    /// <summary>
    /// Last device error reported
    /// </summary>
    string LastDeviceError { get; set; }

    /// <summary>
    /// Method to receive a state changed event fired request
    /// </summary>
    /// <param name="requestData">Current request data</param>
    void StateChangedNotificationReceived(StateChangedEventFiredBusinessTransactionRequestData requestData);

    /// <summary>
    /// Method to receive a report device error request
    /// </summary>
    /// <param name="requestData">Current request data</param>
    void ReportDeviceErrorReceived(ErrorBusinessTransactionRequestData requestData);
}



