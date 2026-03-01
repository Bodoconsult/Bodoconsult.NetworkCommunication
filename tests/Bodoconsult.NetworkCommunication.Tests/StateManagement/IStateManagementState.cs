// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement;

/// <summary>
/// State managament state
/// </summary>
public interface IStateManagementState
{
    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementContext CurrentContext { get; set; }

    string Name { get; }

    bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Allowed next states
    /// </summary>
    List<string> AllowedNextStates { get; }

    /// <summary>
    /// Read only list all orders required by this state
    /// </summary>
    List<IOrder> Orders { get; }

    /// <summary>
    /// The next state to run after all orders for this state were running
    /// </summary>
    IStateManagementState NextState { get; set; }

    /// <summary>
    /// Initiate the state i.e. with creating required orders
    /// </summary>
    void InitiateState();

    /// <summary>
    /// Ser the initial states for this state
    /// </summary>
    void SetInitalStates();

    /// <summary>
    /// Run the next order for the state
    /// </summary>
    void RunNextOrder();

    /// <summary>
    /// Handle a ComDevClose event
    /// </summary>
    void HandleComDevClose();

    /// <summary>
    /// Handle an error message sent by the device
    /// </summary>
    /// <param name="message">Error message</param>
    void HandleErrorMessage(IInboundDataMessage message);
}