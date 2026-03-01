// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Base class for <see cref="IStateManagementState"/> instances
/// </summary>
public abstract class StateManagementStateBase: IStateManagementState
{
    public int CurrentOrderId { get; protected set; }
    
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    protected StateManagementStateBase(IStateManagementContext currentContext)
    {
        CurrentContext = currentContext;
        Name = GetType().Name;
    }

    /// <summary>
    /// Current device state
    /// </summary>
    public IDeviceState DeviceState { get; protected set; }

    /// <summary>
    /// Current business state
    /// </summary>
    public IBusinessState BusinessState { get; protected set; }

    /// <summary>
    /// Current business substate
    /// </summary>
    public IBusinessSubState BusinessSubState { get; protected set; }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementContext CurrentContext { get;  }

    /// <summary>
    /// Type name of the order
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Is cancellation of running orders required on error?
    /// </summary>
    public bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Allowed next states
    /// </summary>
    public List<string> AllowedNextStates { get; } = new();

    /// <summary>
    /// Read only list all orders required by this state
    /// </summary>
    public List<IOrder> Orders { get; } = new();

    /// <summary>
    /// The next state to run after all orders for this state were running
    /// </summary>
    public IStateManagementState NextState { get; set; }

    /// <summary>
    /// Initiate the state i.e. with creating required orders
    /// </summary>
    public virtual void InitiateState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ser the initial states for this state
    /// </summary>
    public virtual void SetInitalStates()
    {
        throw new NotSupportedException("Override in derived class");
    }

    /// <summary>
    /// Run the next order for the state
    /// </summary>
    public virtual void RunNextOrder()
    {
        if (Orders.Count == 0)
        {
            return;
        }

        var order = Orders[CurrentOrderId];



        CurrentContext.OrderProcessor.AddOrder(order);
    }

    /// <summary>
    /// The order was finished successfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedSuccessful(IOrder order)
    {
        // Now run an order specific successfully finished action
        if (order.OrderFinishedSuccessfulAction != null)
        {
            // Get the transport object from the last request. May be null
            var tro = order.RequestSpecs[^1].TransportObject;
            order.OrderFinishedSuccessfulAction(tro, order.ParameterSet);
        }

        // Now logging and notifications
        if (order.StateToNotifyOnSuccess != DefaultDeviceStates.DeviceStateOffline)
        {
            CurrentContext.DoNotify(order.StateToNotifyOnSuccess);
        }

        order.ExecutionState = OrderState.FinishedSuccessfully;
        CurrentContext.LogDebug($"{order.LoggerId}has finished successful");
        //MessagingBusinessDelegate?.DoNotifyOrderStateChanged(this, order);
    }

    /// <summary>
    /// The order was NOT finished succesfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedUnsuccessful(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handle a ComDevClose event
    /// </summary>
    public virtual void HandleComDevClose()
    {
        // Do nothing
    }

    /// <summary>
    /// Handle an error message sent by the device
    /// </summary>
    /// <param name="message">Error message</param>
    public virtual void HandleErrorMessage(IInboundDataMessage message)
    {
        // Do nothing
    }

    /// <summary>
    /// Handle an async received message sent by the device
    /// </summary>
    /// <param name="message">Async received message</param>
    public virtual void HandleAsyncMessage(IInboundDataMessage message)
    {
        // Do nothing
    }

    /// <summary>
    /// Create a string for logging
    /// </summary>
    /// <returns>String with state info</returns>
    public virtual string ToLogString()
    {
        return $"DState: {DeviceState.Id} {DeviceState.Name} BState: {BusinessState.Id} {BusinessState.Name} BSubState: {BusinessSubState.Id} {BusinessSubState.Name}";
    }
}