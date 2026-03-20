// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Configurations;

/// <summary>
/// Current implementation of <see cref="IOrderBasedActionStateConfiguration"/>. State machine state configurations have to set per device
/// </summary>
public class OrderBasedActionStateConfiguration : IOrderBasedActionStateConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="stateName">Name of the state to configure</param>
    /// <param name="stateBuilderBuilder">State builder to use</param>
    public OrderBasedActionStateConfiguration(string stateName, IStateMachineStateBuilder stateBuilderBuilder)
    {
        StateName = stateName;
        StateBuilderBuilder  = stateBuilderBuilder;
    }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateMachineDevice? CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    public string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    public IStateMachineStateBuilder StateBuilderBuilder { get; }

    /// <summary>
    /// Delegate to handle a ComDevClose event in business logic
    /// </summary>
    public HandleComDevCloseDelegate? HandleComDevCloseDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    public HandleErrorMessageDelegate? HandleErrorMessageDelegate { get; set; } 

    /// <summary>
    /// Handle an async received message
    /// </summary>
    public HandleAsyncMessageDelegate? HandleAsyncMessageDelegate { get; set; } 

    /// <summary>
    /// Delegate for preparing orders for the regular state reqeust
    /// </summary>
    public PrepareRegularStateRequestDelegate? PrepareRegularStateRequestDelegate { get; set; } 

    /// <summary>
    /// Delegate for handling device state check request answers in business logic
    /// </summary>
    public HandleRegularStateRequestAnswerDelegate? HandleRegularStateRequestAnswerDelegate { get; set; } 


    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedSucessfullyDelegate? OrderFinishedSucessfullyDelegate { get; set; } 

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedUnsucessfullyDelegate? OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// Parameter sets for the orders to be created for the state. The number of parameter sets must equal the number of <see cref="OrderConfigurations"/>
    /// </summary>
    public List<IParameterSet> ParameterSets { get; } = new();

    /// <summary>
    /// All configurations for orders to be executed for the state to be configured. Sort order is important! The first configuration added is executed as first order etc.
    /// The number of <see cref="IOrderBasedActionStateConfiguration.ParameterSets"/> must equal the number of <see cref="IOrderBasedActionStateConfiguration.OrderConfigurations"/>
    /// </summary>
    public List<string> OrderConfigurations { get; } = new();

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        var config = new OrderBasedActionStateConfiguration(StateName, StateBuilderBuilder)
        {
            HandleRegularStateRequestAnswerDelegate = HandleRegularStateRequestAnswerDelegate,
            HandleAsyncMessageDelegate = HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = HandleErrorMessageDelegate,
            PrepareRegularStateRequestDelegate = PrepareRegularStateRequestDelegate,
            OrderFinishedUnsucessfullyDelegate = OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = OrderFinishedSucessfullyDelegate
        };
        config.OrderConfigurations.AddRange(OrderConfigurations);
        return config;
    }

    /// <summary>
    /// Is  a config valid
    /// </summary>
    /// <param name="config">Config to check</param>
    /// <returns>True if the config is valid else false</returns>
    public static List<string> IsValid(IOrderBasedActionStateConfiguration config)
    {
        var result = new List<string>();

        //if (config.PrepareOrdersForStateMachineStateDelegate== null)
        //{
        //    result.Add("PrepareOrdersForStateMachineStateDelegate not configured");
        //}

        //if (config.OrderFinishedSucessfullyDelegate == null)
        //{
        //    result.Add("OrderFinishedSucessfullyDelegate not configured");
        //}

        //if (config.OrderFinishedUnsucessfullyDelegate == null)
        //{
        //    result.Add("OrderFinishedUnsucessfullyDelegate not configured");
        //}
        
        //if (config.HandleAsyncMessageDelegate == null)
        //{
        //    result.Add("HandleAsyncMessageDelegate not configured");
        //}

        //if (config.HandleComDevCloseDelegate == null)
        //{
        //    result.Add("HandleComDevCloseDelegate not configured");
        //}

        //if (config.HandleErrorMessageDelegate == null)
        //{
        //    result.Add("HandleErrorMessageDelegate not configured");
        //}

        //if (config.HandleRegularStateRequestAnswerDelegate == null)
        //{
        //    result.Add("HandleRegularStateRequestAnswerDelegate not configured");
        //}

        //if (config.PrepareOrdersForStateMachineStateDelegate == null)
        //{
        //    result.Add("PrepareOrdersForStateMachineStateDelegate not configured");
        //}

        //if (config.PrepareRegularStateRequestDelegate == null)
        //{
        //    result.Add("PrepareRegularStateRequestDelegate not configured");
        //}

        return result;
    }
}