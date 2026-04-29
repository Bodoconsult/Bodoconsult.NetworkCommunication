// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

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
    protected BaseOrderBasedStateMachineState(IStateMachineDevice currentContext, int id, string name) : base(
        currentContext, id, name)
    {
        CurrentOrderIndex = 0;
    }

    /// <summary>
    /// Current order index. See <see cref="RunNextOrder"/>
    /// </summary>
    public int CurrentOrderIndex { get; protected set; }

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

        try
        {
            var orderFactory = CurrentContext.OrderManager.OrderFactory;

            for (var index = 0; index < OrderConfigurations.Count; index++)
            {
                var ps = ParameterSets[index];
                
                var orderConfigName = OrderConfigurations[index];
                var order = orderFactory.CreateOrder(orderConfigName, ps);
                //Trace.TraceInformation($"{Name}: PS order ID {ps.CurrentOrder?.Id ?? 0}...");
                Orders.AddRange(order);
                //Trace.TraceInformation($"{Name}: order ID {ps.CurrentOrder?.Id ?? 0} with index {index} was created!");
            }
        }
        catch (Exception e)
        {
            var msg = $"{e}";
            Trace.TraceInformation(msg);
            Trace.TraceError($"BaseOrderBasedStateMachineState :{msg}");
            throw;
        }
    }

    /// <summary>
    /// Parametersets for the orders to be created for the state. The number of parametersets must equal the number of <see cref="IOrderBasedActionStateMachineState.OrderConfigurations"/>
    /// </summary>
    public List<IParameterSet> ParameterSets { get; } = [];

    /// <summary>
    /// All configurations for orders to be executed for the state to be configured. Sort order is important! The first configuration added is executed as first order etc.
    /// </summary>
    public List<string> OrderConfigurations { get; } = [];

    /// <summary>
    /// Orders to be handled by the current state
    /// </summary>
    public List<IOrder> Orders { get; } = [];

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

        if (CurrentOrderIndex >= Orders.Count)
        {
            return;
        }

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
        try
        {
            // Find order
            var order = Orders.FirstOrDefault(x => x.Id == orderId);

            if (order == null)
            {
                return;
            }

            // Now call business logic
            OrderFinishedSucessfullyDelegate?.Invoke(this, order);

            if (CurrentOrderIndex < Orders.Count)
            {
                RunNextOrder();
                return;
            }

            if (NextState == null)
            {
                if (string.IsNullOrEmpty(StateNameOnSuccess))
                {
                    ArgumentNullException.ThrowIfNull(NextState);
                }

                if (CurrentContext.StateMachineStateFactory == null)
                {
                    return;
                }

                ArgumentNullException.ThrowIfNull(StateNameOnSuccess);

                var newState = CurrentContext.CreateStateInstance(StateNameOnSuccess);
                NextState = newState;
            }

            RequestNextState();
        }
        catch (Exception e)
        {
            Trace.TraceInformation(e.ToString());
        }
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

    /// <summary>
    /// Name of the next state for the default
    /// </summary>
    public string? StateNameOnSuccess { get; set; }
}