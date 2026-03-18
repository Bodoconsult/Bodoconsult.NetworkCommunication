// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Base class for <see cref="IStateMachineState"/> implementations
/// </summary>
public abstract class BaseOrderBasedStateMachineState : BaseStateMachineState, IOrderBasedActionStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    protected BaseOrderBasedStateMachineState(IStateManagementDevice currentContext, int id, string name) : base(currentContext, id, name)
    { }

    /// <summary>
    /// Initiate this state
    /// </summary>
    public override void InitiateState()
    {
        if (ParameterSets.Count != OrderConfigurations.Count)
        {
            throw new ArgumentException($"The number of parametersets {ParameterSets.Count} must equal the number of order configurations {OrderConfigurations.Count}!");
        }
        ArgumentNullException.ThrowIfNull(CurrentContext.OrderManager);

        var orderFactory = CurrentContext.OrderManager.OrderFactory;

        for (var index = 0; index < OrderConfigurations.Count; index++)
        {
            var orderConfigName = OrderConfigurations[index];
            var order = orderFactory.CreateOrder(orderConfigName, ParameterSets[index]);
            Orders.AddRange(order);
        }
    }

    /// <summary>
    /// Parametersets for the orders to be created for the state. The number of parametersets must equal the number of <see cref="IOrderBasedActionStateMachineState.OrderConfigurations"/>
    /// </summary>
    public List<IParameterSet> ParameterSets { get; } = new();

    /// <summary>
    /// All configurations for orders to be executed for the state to be configured. Sort order is important! The first configuration added is executed as first order etc.
    /// </summary>
    public List<string> OrderConfigurations { get; } = new();

    /// <summary>
    /// Orders to be handled by the current state
    /// </summary>
    public List<IOrder> Orders { get; } = new();

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    public virtual void RunNextOrder()
    {
        if (Orders.Count == 0)
        {
            throw new ArgumentException("No orders loaded. Call InitiateState() before RunNextOrder()");
        }

        ArgumentNullException.ThrowIfNull(CurrentContext.OrderManager);

        var order = Orders[CurrentOrderIndex];

        //if (order == null)
        //{
        //    return;
        //}

        // Update index before adding the order. Otherwise unit tests fail (due to faking out async behavior)
        CurrentOrderIndex++;

        if (order.IsHighPriorityOrder)
        {
            CurrentContext.OrderManager.OrderProcessor.AddOrderWithPriority(order);
        }
        else
        {
            CurrentContext.OrderManager.OrderProcessor.AddOrder(order);
        }
    }

    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedSucessfullyDelegate? OrderFinishedSucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    public OrderFinishedUnsucessfullyDelegate? OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    public virtual void OrderFinishedSucessfully(long orderId)
    {
        // Find order
        var order = Orders.FirstOrDefault(x => x.Id == orderId);

        if (order == null)
        {
            return;
        }

        // Now call business logic
        OrderFinishedSucessfullyDelegate?.Invoke(this, order);
    }

    /// <summary>
    /// The order has been finished unsuccessfully. Base implementations sets back to online state
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    public virtual void OrderFinishedUnsucessfully(long orderId)
    {
        // Save current job state if necessary
        CurrentContext.SaveJobState(this);

        // Reset internal state
        CurrentContext.ResetInternalState();

        // Find order
        var order = Orders.FirstOrDefault(x => x.Id == orderId);

        if (order == null)
        {
            return;
        }

        // Now call business logic
        OrderFinishedUnsucessfullyDelegate?.Invoke(this, order);
    }
}