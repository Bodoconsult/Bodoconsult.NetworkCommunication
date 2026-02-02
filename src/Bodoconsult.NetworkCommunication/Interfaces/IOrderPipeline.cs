// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing order pipelines for TOM
/// </summary>
public interface IOrderPipeline : IDisposable
{
    /// <summary>
    /// Delegate for sending a notification if the order state has changed
    /// </summary>
    DoNotifyOrderStateChangedDelegate DoNotifyOrderStateChangedDelegate { get; set; }

    /// <summary>
    /// All currently running request processors at the current timepoint
    /// </summary>
    IList<IRequestProcessor> RunningRequestProcessors { get; }

    /// <summary>
    /// All currently running and not cancelled orders at the current timepoint
    /// </summary>
    IList<IOrder> RunningOrders { get; }

    /// <summary>
    /// Number of order running
    /// </summary>
    int RunningOrdersCount { get; }

    /// <summary>
    /// Waiting non-priority and not cancelled orders
    /// </summary>
    IList<IOrder> WaitingOrders { get; }

    /// <summary>
    /// Waiting priority and not cancelled orders
    /// </summary>
    IList<IOrder> WaitingPriorityOrders { get; }

    /// <summary>
    /// All executed order at the current timepoint
    /// </summary>
    IList<IOrder> AllRunningOrders { get; }

    /// <summary>
    /// Waiting non-priority orders
    /// </summary>
    IList<IOrder> AllWaitingOrders { get; }

    /// <summary>
    /// Waiting priority orders
    /// </summary>
    IList<IOrder> AllWaitingPriorityOrders { get; }

    /// <summary>
    /// Show the current order state. Public for unit testing
    /// </summary>
    string CurrentOrderState { get; }

    /// <summary>
    /// Is the queue with running and NOT cancelled orders queue empty
    /// </summary>
    bool IsRunningOrdersEmpty { get; }

    /// <summary>
    /// Is queue with the waiting non-priority and NOT cancelled orders empty
    /// </summary>
    bool IsWaitingOrdersEmpty { get; }

    /// <summary>
    /// Is queue with the waiting priority and NOT cancelled orders empty
    /// </summary>
    bool IsWaitingPriorityOrdersEmpty { get; }

    /// <summary>
    /// Is a certain order type running or waiting for execution
    /// </summary>
    /// <param name="orderTypeCode">Order type code: see <see cref="OrderTypeCodes"/> for valid values</param>
    /// <returns>True if an order is running or waiting in the queue else false</returns>
    bool IsOrderTypeInTheQueue(int orderTypeCode);

    /// <summary>
    /// Add a non-priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    void AddOrder(IOrder order);

    /// <summary>
    /// Add a priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    void AddPriorityOrder(IOrder order);

    /// <summary>
    /// Dequeue the order to run from priority and non-priority waiting queues
    /// </summary>
    /// <param name="order">Current order to run</param>
    /// <returns>True if the order could not be dequeued else false</returns>
    bool DequeueOrder(IOrder order);

    /// <summary>
    /// Check the execution queue for hanging messages
    /// </summary>
    void RemoveCancelledOrdersFromExecutionQueue();

    /// <summary>
    /// Cancel all queued priority orders
    /// </summary>
    void CancelWaitingPriorityOrders();

    /// <summary>
    /// Cancel all queued non-priority orders
    /// </summary>
    void CancelWaitingOrders();

    /// <summary>
    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
    /// </summary>
    void CancelWaitingOrdersOnComDevClose();

    /// <summary>
    /// Cancel all orders bound to a certain source item like a joblist
    /// </summary>
    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
    void CancelOrderBySourceUid(Guid sourceUid);

    /// <summary>
    /// Cancel a order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    void CancelOrder(long orderId);

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    void CancelOrder(IOrder order);

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    /// <param name="errorCode">Error code to set for the order</param>
    void CancelOrder(IOrder order, int errorCode);

    /// <summary>
    /// Get the next priority order
    /// </summary>
    /// <returns>Priority order or null</returns>
    IOrder GetNextPriorityOrder();

    /// <summary>
    /// Get the next non-priority order
    /// </summary>
    /// <returns>Non-priority order or null</returns>
    IOrder GetNextNonPriorityOrder();

    /// <summary>
    /// Prepare the order start (but do NOT start it). Public only for testing
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="orderProcessingFinishedDelegate">Delegate called when the order has finished</param>
    /// <param name="p">Request processor running the order. To be created</param>
    /// <returns>True if preparing the order failed else false</returns>
    bool PrepareOrderStart(IOrder order, OrderProcessingFinishedDelegate orderProcessingFinishedDelegate, out IRequestProcessor p);

    /// <summary>
    /// Execute the request processor with its order
    /// </summary>
    /// <param name="requestProcessor">Current request processor to execute</param>
    void ExecuteOrder(IRequestProcessor requestProcessor);

    /// <summary>
    /// Get an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Removed request processor</returns>
    IRequestProcessor GetFromExecutionQueue(long orderId);


    /// <summary>
    /// Remove an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Removed request processor</returns>
    void RemoveFromExecutionQueue(long orderId);

    /// <summary>
    /// Check if a certain order is running currently by its ID
    /// </summary>
    /// <param name="orderId">Requested order ID</param>
    /// <returns>True if the order is running currently else false</returns>
    bool CheckIfOrderIsRunning(long orderId);
}