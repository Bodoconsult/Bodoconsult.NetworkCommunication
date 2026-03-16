// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Fake implemenation of <see cref="IOrderPipeline"/>
/// </summary>
public class FakeOrderPipeline : IOrderPipeline
{
    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Delegate for sending a notification if the order state has changed
    /// </summary>
    public DoNotifyOrderStateChangedDelegate? DoNotifyOrderStateChangedDelegate { get; set; } 

    /// <summary>
    /// All currently running request processors at the current timepoint
    /// </summary>
    public IList<IRequestProcessor> RunningRequestProcessors => new List<IRequestProcessor>();

    /// <summary>
    /// All currently running and not cancelled orders at the current timepoint
    /// </summary>
    public IList<IOrder> RunningOrders => new List<IOrder>();

    /// <summary>
    /// Number of order running
    /// </summary>
    public int RunningOrdersCount { get; set; }

    /// <summary>
    /// Waiting non-priority and not cancelled orders
    /// </summary>
    public IList<IOrder> WaitingOrders => new List<IOrder>();

    /// <summary>
    /// Waiting priority and not cancelled orders
    /// </summary>
    public IList<IOrder> WaitingPriorityOrders => new List<IOrder>();

    /// <summary>
    /// All executed order at the current timepoint
    /// </summary>
    public IList<IOrder> AllRunningOrders => new List<IOrder>();

    /// <summary>
    /// Waiting non-priority orders
    /// </summary>
    public IList<IOrder> AllWaitingOrders => new List<IOrder>();

    /// <summary>
    /// Waiting priority orders
    /// </summary>
    public IList<IOrder> AllWaitingPriorityOrders => new List<IOrder>();

    /// <summary>
    /// Show the current order state. Public for unit testing
    /// </summary>
    public string CurrentOrderState { get; set; } = string.Empty;

    /// <summary>
    /// Is the queue with running and NOT cancelled orders queue empty
    /// </summary>
    public bool IsRunningOrdersEmpty { get; set; }

    /// <summary>
    /// Is queue with the waiting non-priority and NOT cancelled orders empty
    /// </summary>
    public bool IsWaitingOrdersEmpty { get; set; }

    /// <summary>
    /// Is queue with the waiting priority and NOT cancelled orders empty
    /// </summary>
    public bool IsWaitingPriorityOrdersEmpty { get; set; }

    /// <summary>
    /// Is a certain order type running or waiting for execution
    /// </summary>
    /// <param name="orderTypeCode">Order type code</param>
    /// <returns>True if an order is running or waiting in the queue else false</returns>
    public bool IsOrderTypeInTheQueue(int orderTypeCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add a non-priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    public void AddOrder(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add a priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    public void AddPriorityOrder(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Dequeue the order to run from priority and non-priority waiting queues
    /// </summary>
    /// <param name="order">Current order to run</param>
    /// <returns>True if the order could not be dequeued else false</returns>
    public bool DequeueOrder(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check the execution queue for hanging messages
    /// </summary>
    public void RemoveCancelledOrdersFromExecutionQueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all queued priority orders
    /// </summary>
    public void CancelWaitingPriorityOrders()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all queued non-priority orders
    /// </summary>
    public void CancelWaitingOrders()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
    /// </summary>
    public void CancelWaitingOrdersOnComDevClose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all orders bound to a certain source item like a joblist
    /// </summary>
    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
    public void CancelOrderBySourceUid(Guid sourceUid)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel an order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    public void CancelOrder(long orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    public void CancelOrder(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    /// <param name="errorCode">Error code to set for the order</param>
    public void CancelOrder(IOrder order, int errorCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the next priority order
    /// </summary>
    /// <returns>Priority order or null</returns>
    public IOrder? GetNextPriorityOrder()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the next non-priority order
    /// </summary>
    /// <returns>Non-priority order or null</returns>
    public IOrder? GetNextNonPriorityOrder()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Prepare the order start (but do NOT start it). Public only for testing
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="orderProcessingFinishedDelegate">Delegate called when the order has finished</param>
    /// <param name="rp">Request processor running the order. To be created</param>
    /// <returns>True if preparing the order failed else false</returns>
    public bool PrepareOrderStart(IOrder order, OrderProcessingFinishedDelegate orderProcessingFinishedDelegate,
        out IRequestProcessor rp)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Execute the request processor with its order
    /// </summary>
    /// <param name="requestProcessor">Current request processor to execute</param>
    public void ExecuteOrder(IRequestProcessor requestProcessor)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Removed request processor</returns>
    public IRequestProcessor? GetFromExecutionQueue(long orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Remove an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Removed request processor</returns>
    public void RemoveFromExecutionQueue(long orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if a certain order is running currently by its ID
    /// </summary>
    /// <param name="orderId">Requested order ID</param>
    /// <returns>True if the order is running currently else false</returns>
    public bool CheckIfOrderIsRunning(long orderId)
    {
        throw new NotImplementedException();
    }
}