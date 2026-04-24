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
    /// State message to show in the UI
    /// </summary>
    public string StateMessage { get; set; }

    /// <summary>
    /// Method to receive a state changed event fired request
    /// </summary>
    /// <param name="requestData">Current request data</param>
    void StateChangedNotificationReceived(StateChangedEventFiredBusinessTransactionRequestData requestData);

}



