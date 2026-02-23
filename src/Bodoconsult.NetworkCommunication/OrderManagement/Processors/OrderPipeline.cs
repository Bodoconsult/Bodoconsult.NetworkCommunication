// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

public class OrderPipeline : IOrderPipeline
{
    private readonly IDateTimeService _dateTimeService;

    private readonly IRequestProcessorFactory _requestProcessorFactory;

    private readonly IAppLoggerProxy _appLogger;

    private readonly string _loggerId;

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderPipeline(IDateTimeService dateTimeService, IRequestProcessorFactory requestProcessorFactory, IAppLoggerProxy appLogger, string loggerId)
    {
        _dateTimeService = dateTimeService;
        _requestProcessorFactory = requestProcessorFactory;
        _appLogger = appLogger;
        _loggerId = loggerId;
    }

    /// <summary>
    /// The internal queue to hold the messages until they are processed
    /// </summary>
    private readonly ConcurrentQueue<IOrder> _orderQueue = new();

    /// <summary>
    /// The internal queue to hold the messages with higher priority until they are processed
    /// </summary>
    private readonly ConcurrentQueue<IOrder> _ordersWithPriorityQueue = new();



    /// <summary>
    /// The current execution list of orders.
    /// Do not access ExecutionQueue directly.
    /// Always take a "copy" of the list with i.e. _executionQueue.Select or _executionQueue.ToList to avoid multithreading iusses
    /// </summary>
    private readonly ConcurrentDictionary<long, IRequestProcessor> _executionQueue = new();


    /// <summary>
    /// Delegate for sending a notification if the order state has changed
    /// </summary>
    public DoNotifyOrderStateChangedDelegate DoNotifyOrderStateChangedDelegate { get; set; }

    /// <summary>
    /// All currently running request processors at the current timepoint
    /// </summary>
    public IList<IRequestProcessor> RunningRequestProcessors => _executionQueue.Values.ToList();

    /// <summary>
    /// All currently running and not cancelled orders at the current timepoint
    /// </summary>
    public IList<IOrder> RunningOrders => UncancelledCurrentlyExecutedOrders;

    /// <summary>
    /// Number of order running
    /// </summary>
    public int RunningOrdersCount => UncancelledCurrentlyExecutedOrders.Count;

    /// <summary>
    /// Waiting non-priority and not cancelled orders
    /// </summary>
    public IList<IOrder> WaitingOrders => _orderQueue.Where(x => !x.IsCancelled).ToList();

    /// <summary>
    /// Waiting priority and not cancelled orders
    /// </summary>
    public IList<IOrder> WaitingPriorityOrders => _ordersWithPriorityQueue.Where(x => !x.IsCancelled).ToList();

    /// <summary>
    /// All executed order at the current timepoint
    /// </summary>
    public IList<IOrder> AllRunningOrders => _executionQueue.Values.Select(x => x.Order).ToList();

    /// <summary>
    /// Waiting non-priority orders
    /// </summary>
    public IList<IOrder> AllWaitingOrders => _orderQueue.ToList();

    /// <summary>
    /// Waiting priority orders
    /// </summary>
    public IList<IOrder> AllWaitingPriorityOrders => _ordersWithPriorityQueue.ToList();

    public List<IOrder> UncancelledCurrentlyExecutedOrders
    {
        get
        {
            var result = _executionQueue.Values.Select(x => x.Order).Where(x => x is { IsCancelled: false }).ToList();
            return result;
        }
    }


    /// <summary>
    /// Is a certain order type running or waiting for execution
    /// </summary>
    /// <param name="orderTypeCode">Order type code: see <see cref="OrderTypeCodes"/> for valid values</param>
    /// <returns>True if an order is running or waiting in the queue else false</returns>
    public bool IsOrderTypeInTheQueue(int orderTypeCode)
    {
        return UncancelledCurrentlyExecutedOrders.Any(order => order.OrderTypeCode == orderTypeCode) ||
               _ordersWithPriorityQueue.Any(order => order.OrderTypeCode == orderTypeCode && !order.IsCancelled) ||
               _orderQueue.Any(order => order.OrderTypeCode == orderTypeCode && !order.IsCancelled);
    }

    /// <summary>
    /// Show the current order state. Public for unit testing
    /// </summary>
    public string CurrentOrderState => $"OrderQ {_orderQueue.Count} PrioQ {_ordersWithPriorityQueue.Count} ExecQ {UncancelledCurrentlyExecutedOrders.Count}";

    /// <summary>
    /// Is the queue with running and NOT cancelled orders queue empty
    /// </summary>
    public bool IsRunningOrdersEmpty => _executionQueue.IsEmpty || !UncancelledCurrentlyExecutedOrders.Any();

    /// <summary>
    /// Is queue with the waiting non-priority and NOT cancelled orders empty
    /// </summary>
    public bool IsWaitingOrdersEmpty => _orderQueue.IsEmpty || _orderQueue.All(x => x.IsCancelled);

    /// <summary>
    /// Is queue with the waiting priority and NOT cancelled orders empty
    /// </summary>
    public bool IsWaitingPriorityOrdersEmpty => _ordersWithPriorityQueue.IsEmpty || _ordersWithPriorityQueue.All(x => x.IsCancelled);



    /// <summary>
    /// Add a non-priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    public void AddOrder(IOrder order)
    {
        if (order.IsHighPriorityOrder)
        {
            throw new ArgumentException($"High priority order {order.Id} NOT allowed to run as a normal order");
        }
        _orderQueue.Enqueue(order);
        order.ExecutionState = OrderState.Added;
        _appLogger.LogInformation($"{_loggerId}{order.LoggerId}added to queue: {JsonHelper.JsonSerialize(order.ParameterSet)}");
    }

    /// <summary>
    /// Add a priority order
    /// </summary>
    /// <param name="order">Order to add to the pipeline</param>
    public void AddPriorityOrder(IOrder order)
    {
        if (!order.IsHighPriorityOrder)
        {
            throw new ArgumentException($"Normal order {order.Id} NOT allowed to run as a high priority order");
        }

        _ordersWithPriorityQueue.Enqueue(order);
        order.ExecutionState = OrderState.Added;
        _appLogger.LogInformation($"{_loggerId}{order.LoggerId}added to queue: {JsonHelper.JsonSerialize(order.ParameterSet)}");
    }

    /// <summary>
    /// Dequeue the order to run from priority and non-priority waiting queues
    /// </summary>
    /// <param name="order">Current order to run</param>
    /// <returns>True if the order could not be dequeued else false</returns>
    public bool DequeueOrder(IOrder order)
    {
        if (_orderQueue.Contains(order))
        {
            _orderQueue.TryDequeue(out _);
        }
        else if (_ordersWithPriorityQueue.Contains(order))
        {
            _ordersWithPriorityQueue.TryDequeue(out _);
        }
        else
        {
            var s = $"{_loggerId}next order could not be dequeued from OrderQueue";
            _appLogger.LogError(s);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check the execution queue for hanging messages
    /// </summary>
    public void RemoveCancelledOrdersFromExecutionQueue()
    {
        try
        {
            if (_executionQueue.IsEmpty)
            {
                return;
            }

            foreach (var proc in _executionQueue.ToList())
            {
                var order = proc.Value?.Order;
                if (order is not { IsCancelled: true })
                {
                    continue;
                }

                var success = _executionQueue.TryRemove(proc.Key, out var unUsed);
                if (success)
                {
                    unUsed.Cancel(true, false);
                }

                // Break loop now if order was not cancelled and order is not a dummy order
                if (order.TypeName == nameof(DummyOrder))
                {
                    continue;
                }

                order.ExecutionState = OrderState.Canceled;
                DoNotifyOrderStateChangedDelegate?.Invoke(order);
            }
        }
        catch //(Exception e)
        {
            // Do nothing
            //_appLogger.LogError("Removing cancelled orders failed", e);
        }

    }

    /// <summary>
    /// Cancel all queued priority orders
    /// </summary>
    public void CancelWaitingPriorityOrders()
    {
        if (_ordersWithPriorityQueue.IsEmpty)
        {
            return;
        }

        try
        {
            foreach (var order in _ordersWithPriorityQueue.Where(x => !x.IsCancelled).ToList())
            {
                if (order == null)
                {
                    continue;
                }
                order.IsCancelled = true;
                CancelOrderInternal(order);
            }
        }
        catch (Exception e)
        {
            _appLogger.LogError("cancelling priority orders failed", e);
        }

    }

    /// <summary>
    /// Cancel all queued non-priority orders
    /// </summary>
    public void CancelWaitingOrders()
    {
        if (_orderQueue.IsEmpty)
        {
            return;
        }

        try
        {
            foreach (var order in _orderQueue.Where(x => !x.IsCancelled).ToList())
            {
                if (order == null)
                {
                    continue;
                }
                CancelOrderInternal(order);
            }
        }
        catch (Exception e)
        {
            _appLogger.LogError("cancelling non-priority orders failed", e);
        }
    }

    /// <summary>
    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
    /// </summary>
    public void CancelWaitingOrdersOnComDevClose()
    {
        if (_orderQueue.IsEmpty)
        {
            return;
        }

        try
        {
            foreach (var order in _orderQueue.Where(x => !x.IsCancelled).ToList())
            {
                if (order is not { IsCancelledOnComDevClose: true })
                {
                    continue;
                }

                CancelOrderInternal(order);
            }
        }
        catch (Exception e)
        {
            _appLogger.LogError("cancelling non-priority orders failed", e);
        }
    }

    /// <summary>
    /// Cancel all orders bound to a certain source item like a joblist
    /// </summary>
    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
    public void CancelOrderBySourceUid(Guid sourceUid)
    {
        // Check the current running orders for orders bound to sourceUid
        foreach (var proc in _executionQueue.Values)
        {
            if (proc.Order.SourceUid != sourceUid)
            {
                continue;
            }

            if (!proc.IsCancelled)
            {
                proc.Cancel(true, false);
            }
            CancelOrderInternal(proc.Order);
            return;
        }

        // Now check the priority queue for the order
        foreach (var item in _ordersWithPriorityQueue.Where(x => !x.IsCancelled).ToList())
        {
            if (item.SourceUid == sourceUid)
            {
                continue;
            }
            CancelOrderInternal(item);
            return;
        }

        // Now check the queue for the order
        foreach (var item in _orderQueue.Where(x => !x.IsCancelled).ToList())
        {
            if (item.SourceUid == sourceUid)
            {
                continue;
            }
            CancelOrderInternal(item);
            return;
        }
    }


    /// <summary>
    /// Cancel a order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    public void CancelOrder(long orderId)
    {
        if (orderId == 0)
        {
            return;
        }

        // Check the current running orders
        foreach (var proc in _executionQueue.Values.ToList())
        {
            var order = proc.Order;

            if (order == null )
            {
                if (!proc.IsCancelled)
                {
                    proc.Cancel(true, false);
                }
                continue;
            }

            if (order.Id != orderId)
            {
                continue;
            }

            if (!proc.IsCancelled)
            {
                proc.Cancel(true, false);
            }

            CancelOrderInternal(order);

            return;
        }

        // Now check the priority queue for the order
        foreach (var item in _ordersWithPriorityQueue.Where(x => !x.IsCancelled).ToList())
        {
            if (item.Id != orderId)
            {
                continue;
            }

            CancelOrderInternal(item);
            return;
        }

        // Now check the queue for the order
        foreach (var item in _orderQueue.Where(x => !x.IsCancelled).ToList())
        {
            if (item.Id != orderId)
            {
                continue;
            }

            CancelOrderInternal(item);
            return;
        }
    }

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    public void CancelOrder(IOrder order)
    {
        CancelOrder(order, 0);
    }

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    /// <param name="errorCode">Error code to set for the order</param>
    public void CancelOrder(IOrder order, int errorCode)
    {
        if (order == null)
        {
            return;
        }

        var proc = _executionQueue.Values.ToList().FirstOrDefault(x => x.Order!= null && x.Order.Id == order.Id && !x.Order.IsCancelled);

        // Cancel running order
        if (proc != default)
        {
            try
            {
                if (!proc.IsCancelled)
                {
                    proc.Cancel(true, false);
                }
            }
            catch //(Exception e)
            {
                // Do nothing
            }
        }

        CancelOrderInternal(order);
        order.OrderError = errorCode;
    }

    private void CancelOrderInternal(IOrder order)
    {
        var o = order;

        if (o == null || o.IsCancelled)
        {
            return;
        }

        try
        {
            o.IsCancelled = true;
            o.ExecutionResult = OrderExecutionResultState.Unsuccessful;
            //_deviceServer.deviceOrderManager.StSysdeviceBusinessState = StSysdeviceHardwareState.OrderProcessingError;
            _appLogger.LogInformation($"{_loggerId}{o.LoggerId}: order cancelled");
        }
        catch (Exception e)
        {
            _appLogger.LogError($"Cancelling order {o.Id} failed", e);
        }

    }

    /// <summary>
    /// Get the next priority order
    /// </summary>
    /// <returns>Priority order or null</returns>
    public IOrder GetNextPriorityOrder()
    {
        while (true)
        {
            bool success;
            IOrder order;

            if (!_ordersWithPriorityQueue.IsEmpty)
            {
                success = _ordersWithPriorityQueue.TryPeek(out order);
            }
            else
            {
                return null;
            }

            if (!success || order == null)
            {
                return null;
            }

            // Break loop now if order was not cancelled and order is not a dummy order
            if (!order.IsCancelled && order.TypeName != nameof(DummyOrder))
            {
                return order;
            }

            order.ExecutionState = OrderState.Canceled;
            DoNotifyOrderStateChangedDelegate?.Invoke(order);
            _ordersWithPriorityQueue.TryDequeue(out order);
        }
    }

    /// <summary>
    /// Get the next non-priority order
    /// </summary>
    /// <returns>Non-priority order or null</returns>
    public IOrder GetNextNonPriorityOrder()
    {
        while (true)
        {
            bool success;
            IOrder order;

            if (!_orderQueue.IsEmpty)
            {
                success = _orderQueue.TryPeek(out order);
            }
            else
            {
                return null;
            }

            if (!success || order == null)
            {
                return null;
            }

            // Break loop now if order was not cancelled and order is not a dummy order
            if (!order.IsCancelled
                && order.TypeName != nameof(DummyOrder))
            {
                return order;
            }

            order.ExecutionState = OrderState.Canceled;
            DoNotifyOrderStateChangedDelegate?.Invoke(order);
            _orderQueue.TryDequeue(out order);
        }
    }

    /// <summary>
    /// Prepare the order start (but do NOT start it). Public only for testing
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="orderProcessingFinishedDelegate">Delegate called when the order has finished</param>
    /// <param name="p">Request processor running the order. To be created</param>
    /// <returns>True if preparing the order failed else false</returns>
    public bool PrepareOrderStart(IOrder order, OrderProcessingFinishedDelegate orderProcessingFinishedDelegate, out IRequestProcessor p)
    {
        p = _requestProcessorFactory.CreateRequestProcessor(order);
        p.OrderProcessingFinishedDelegate = orderProcessingFinishedDelegate;

        if (_executionQueue.TryAdd(order.Id, p))
        {
            return false;
        }

        var s = $"{_loggerId}next order {order.Id} could not be added to ExecutionQueue";
        _appLogger.LogError(s);
        return true;

    }

    /// <summary>
    /// Execute the request processor with its order
    /// </summary>
    /// <param name="requestProcessor">Current request processor to execute</param>
    public void ExecuteOrder(IRequestProcessor requestProcessor)
    {
        requestProcessor.PrepareTheChain();


        var order = requestProcessor.Order;
        order.StartTime = _dateTimeService.Now;


        var task = Task.Run(() =>
        {
            requestProcessor.ExecuteOrder();
        }, requestProcessor.CancellationTokenSource.Token);

        requestProcessor.CurrentTask = task;

        _appLogger.LogInformation($"{_loggerId}{requestProcessor.Order.LoggerId}execution started: {JsonHelper.JsonSerialize(requestProcessor.Order.ParameterSet)}");

        order.ExecutionState = OrderState.Started;
    }

    /// <summary>
    /// Get an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Removed request processor</returns>
    public IRequestProcessor GetFromExecutionQueue(long orderId)
    {
        var success = _executionQueue.TryGetValue(orderId, out var requestProcessor);
        if (success)
        {
            return requestProcessor;
        }

        _appLogger.LogWarning($"{_loggerId}RequestProcessor could not be removed");
        return null;
    }


    /// <summary>
    /// Remove an order request processor from the execution queue by order ID
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    public void RemoveFromExecutionQueue(long orderId)
    {
        var success = _executionQueue.TryRemove(orderId, out _);
        if (!success)
        {
            _appLogger.LogWarning($"{_loggerId}RequestProcessor could not be removed");
        }
    }

    /// <summary>
    /// Check if a certain order is running currently by its ID
    /// </summary>
    /// <param name="orderId">Requested order ID</param>
    /// <returns>True if the order is running currently else false</returns>
    public bool CheckIfOrderIsRunning(long orderId)
    {
        try
        {
            return _executionQueue.ContainsKey(orderId);
        }
        catch (Exception e)
        {
            _appLogger.LogError($"{_loggerId}checking if order {orderId} is running failed", e);
            return false;
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        try
        {
            _ordersWithPriorityQueue.Clear();
            _orderQueue.Clear();
            _executionQueue.Clear();
        }
        catch (Exception e)
        {
            _appLogger.LogError($"{_loggerId}disposing order pipeline failed", e);
        }
    }
}