// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;

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
    /// Device state message to show in the UI
    /// </summary>
    public string DeviceStateMessage { get; set; }

    /// <summary>
    /// Backend state message to show in the UI
    /// </summary>
    public string BackendStateMessage { get; set; }

    /// <summary>
    /// Method to receive a state changed event fired request
    /// </summary>
    /// <param name="requestData">Current request data</param>
    void StateChangedNotificationReceived(StateChangedEventFiredBusinessTransactionRequestData requestData);

}



