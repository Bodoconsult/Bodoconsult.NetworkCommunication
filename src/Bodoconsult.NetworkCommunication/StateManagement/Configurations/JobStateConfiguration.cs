// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Configurations;

/// <summary>
/// Current implementation of <see cref="IJobStateConfiguration"/>. State machine state configurations have to set per device
/// </summary>
public class JobStateConfiguration : IJobStateConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="stateName">Name of the state to configure</param>
    public JobStateConfiguration(string stateName)
    {
        StateName = stateName;
    }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementDevice CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    public string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    public IStateMachineStateBuilder StateBuilderBuilder { get; set; }

    /// <summary>
    /// Delegate to handle a ComDevClose event in business logic
    /// </summary>
    public HandleComDevCloseDelegate HandleComDevCloseDelegate { get; set; } 

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    public HandleErrorMessageDelegate HandleErrorMessageDelegate { get; set; } 

    /// <summary>
    /// Handle an async received message
    /// </summary>
    public HandleAsyncMessageDelegate HandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Delegate for preparing orders for the regular state reqeust
    /// </summary>
    public PrepareRegularStateRequestDelegate PrepareRegularStateRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling device state check request answers in business logic
    /// </summary>
    public HandleRegularStateRequestAnswerDelegate HandleRegularStateRequestAnswerDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedSucessfullyDelegate OrderFinishedSucessfullyDelegate { get; set; } 

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedUnsucessfullyDelegate OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate to create one or more orders sent to device needed for an order based state machine state
    /// </summary>
    public PrepareOrdersForStateMachineStateDelegate PrepareOrdersForStateMachineStateDelegate { get; set; } 

    /// <summary>
    /// The UID of a source item like a joblist or a trial run the order is bound to
    /// </summary>
    public Guid? SourceUid { get; set; }
}