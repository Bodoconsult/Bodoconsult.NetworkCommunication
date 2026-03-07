// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections;
using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Base class for <see cref="IStSysStateMachineState"/> implementations
/// </summary>
public abstract class BaseStateMachineState : IStateMachineState
{
    protected int CurrentOrderIndex;
    private bool _isErrorHandlingRunning;
    private readonly Lock _isErrorHandlingRunningLock = new();

    protected bool IsErrorHandlingRunning
    {
        get
        {
            lock (_isErrorHandlingRunningLock)
            {
                return _isErrorHandlingRunning;
            }
        }
        set
        {
            lock (_isErrorHandlingRunningLock)
            {
                _isErrorHandlingRunning = value;
            }
        }
    }

    public int Id { get; protected set; }

    /// <summary>
    /// Name of the state
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// The states allowed to follow the current state
    /// </summary>
    public List<string> AllowedNextStates { get; protected set; }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementDevice CurrentContext { get; protected set; }

    /// <summary>
    /// Is the cancellation of all running orders required before the state is applied
    /// </summary>
    public bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Is turning offstate request messages to the tower required
    /// </summary>
    public bool IsTurningOffStateRequestsRequired { get; set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseStateMachineState(IStateManagementDevice currentContext)
    {
        CurrentContext = currentContext;
        CurrentOrderIndex = 0;
    }

    /// <summary>
    /// Orders to be handled by the current state
    /// </summary>
    public List<IOrder> Orders { get; } = new();

    /// <summary>
    /// The next state to be requested when this state has to be left or null if the state does not change
    /// </summary>
    public IStateMachineState NextState { get; protected set; }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    public virtual void SetInitalStates()
    {
        Debug.Print("BaseStateMachineState.SetInitialStates: not supported");
        throw new NotSupportedException();
    }

    /// <summary>
    /// Initiate this state
    /// </summary>
    public virtual void InitiateState()
    {
        // Do nothing
    }

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    public virtual void RunNextOrder()
    {
        if (Orders.Count == 0)
        {
            throw new ArgumentException("No orders loaded. Call InitiateState() before RunNextOrder()");
        }

        var order = Orders[CurrentOrderIndex];

        if (order == null)
        {
            return;
        }

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
    /// Cancel this state
    /// </summary>
    public virtual void CancelState()
    {
        // Do nothing
    }

    /// <summary>
    /// Request the next state set by the current state
    /// </summary>
    public void RequestNextState()
    {
        if (NextState == null)
        {
            return;
        }

        CurrentContext.RequestState(NextState);
    }

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    public virtual void OrderFinishedSucessfully(long orderId)
    {
        // Do nothing
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

        // No go to online mode
        NextState = new TowerOnlineStateV100(CurrentContext);
    }


    /// <summary>
    /// The communication to the device was broken. Handle this event
    /// </summary>
    public virtual void HandleComDevClose()
    {

        // Save current job state if necessary
        CurrentContext.SaveJobState(this);

        // Reset internal state
        CurrentContext.ResetInternalState();

        // Now go to offline
        var state = CurrentContext.CreateStateInstance(nameof(DeviceOfflineState));
        NextState = state;
    }

    /// <summary>
    /// Handle a received error message from the device
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    public virtual void HandleErrorMessage(IInboundDataMessage receivedMessage)
    {
        if (IsErrorHandlingRunning)
        {
            return;
        }

        IsErrorHandlingRunning = true;

        string msg;

        try
        {
            //if (receivedMessage is not SmdTowerDataMessage rm)
            //{
            //    msg = $"wrong message received {receivedMessage.ToInfoString()}";
            //    CurrentTowerServer.LogDebug(msg);
            //    return;
            //}

            //var isHardwareError = TowerHelper.IsHardwareError(rm.Error);

            //// Get the tower state from tower message
            //var state = CurrentTowerServer.GetTowerStateFromDatablock(rm);

            //// Hardware errors during tower init are not handled
            //if (state.Id == StSysTowerHardwareState.TowerStateInitState.Id)
            //{
            //    //case StSysTowerHardwareState.TowerStateInitStateCalibRotor:
            //    //case StSysTowerHardwareState.TowerStateInitStateCheckFw:
            //    msg = $"command X received: state {state} {(isHardwareError ? "hardware error" : "error")} {rm.Error}. Hardware error during hardware init is NOT handled";
            //    CurrentTowerServer.LogDebug(msg);
            //    return;
            //}

            //// Init is in processing currently: no error handling at all
            //if (CurrentContext.OrderProcessor.IsInitInProcessing)
            //{
            //    msg = $"command X received: state {state} {(isHardwareError ? "hardware error" : "error")} {rm.Error}. Error handling cancelled due to currently running init process";
            //    CurrentTowerServer.LogWarning(msg);

            //    return;
            //}

            //// Save current job state if necessary
            //CurrentContext.SaveJobState(this);

            //var es = (TowerErrorState)CurrentContext.CreateStateInstance(nameof(TowerErrorState));

            //es.ErrorCode = rm.Error;
            //es.IsHardwareError = isHardwareError;
            //es.CurrentDeviceState = state;

            //// Request new state
            //NextState = es;
            //RequestNextState();
        }
        catch (Exception e)
        {
            //msg = $"error handling failed: {e}";
            //CurrentTowerServer.LogError(msg);

            //// Save current job state if necessary
            //CurrentContext.SaveJobState(this);

            //// Request new state
            //var state = CurrentTowerServer.CreateStateInstance(nameof(TowerOfflineState));
            //NextState = state;
            //RequestNextState();
        }
        finally
        {
            IsErrorHandlingRunning = true;
        }
    }

    /// <summary>
    /// Handle async sent message from device
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    public virtual MessageHandlingResult HandleAsyncMessage(IInboundDataMessage message)
    {

        // ToDo: what should happen here

        return new MessageHandlingResult();
    }






    /// <summary>
    /// Check a received message from device if a state change is necessary
    /// </summary>
    /// <param name="message">Received message from device</param>
    /// <param name="doNotNotifyClient">Do notify the clients</param>
    public virtual MessageHandlingResult CheckReceivedStateMessage(IInboundDataMessage message,
        bool doNotNotifyClient)
    {
        var result = HandleRegularStateRequestAnswer(message, doNotNotifyClient);
        return result;
    }

    /// <summary>
    /// Request a new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    public virtual void RequestNewDeviceState(IDeviceState newDeviceState)
    {
        CurrentContext.RequestNewDeviceState(newDeviceState);
    }

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    public virtual IList<IOrder> PerpareRegularStateRequest()
    {
        var orders = new List<IOrder>();

        if (CurrentContext.PrepareRegularStateRequestDelegate != null)
        {
            orders.AddRange(CurrentContext.PrepareRegularStateRequestDelegate.Invoke());
        }

        return orders;
    }

    public MessageHandlingResult HandleRegularStateRequestAnswer(IInboundDataMessage receivedMessage, bool doNotNotifyClient)
    {
        // ToDo: what should happen here

        return new MessageHandlingResult();
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Id} {Name}";
    }
}