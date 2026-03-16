// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Benchmarking;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Base class for order processors implementing <see cref="IOrderProcessor"/>
/// </summary>
public abstract class BaseOrderProcessor : IOrderProcessor
{
    /// <summary>
    /// Watchdog to process messages received from device
    /// </summary>
    private readonly IWatchDog _messageProcessingWatchdog;

    /// <summary>
    /// Is the runner stopped. DO NOT use this field internally. Use IsRunnerStopped property instead
    /// </summary>
    protected bool IsRunnerStoppedInternal;
    protected readonly Lock IsRunnerStoppedLock = new();

    /// <summary>
    /// true while the orders of StSys init are handled. Important to NOT start real order processing in <see cref="Runner "/> before checkslot was handled
    /// </summary>
    protected bool InitIsProcessing;
    protected readonly Lock InitIsProcessingLock = new();

    // Check if cancellation of running orders is already running to avoid endless loop with stack exception
    protected bool IsCancellationRunningOrdersInternal;
    protected readonly Lock IsCancellationRunningOrdersLockObject = new();
    protected readonly IAppDateService DateTimeService;
    protected readonly IAppLoggerProxy AppLogger;
    protected readonly string LoggerId;
    protected readonly IAppLoggerProxy MonitorLogger;
    protected readonly IAppBenchProxy AppBenchProxy;

    /// <summary>
    /// Start the request processor to execute the order
    /// </summary>
    /// <param name="requestProcessor">Current request processor</param>
    /// <returns>Task with order execution</returns>
    private void ExecuteOrder(IRequestProcessor requestProcessor)
    {
        var order = requestProcessor.Order;
        //if (order == null)
        //{
        //    return;
        //}

        // Now execute
        OrderPipeline.ExecuteOrder(requestProcessor);

        // Notify the app on order execution?
        if (order.IsClientNotificationTurnedOff)
        {
            // No notification
            return;
        }

        // Notify async
        ClientNotificationManager.DoNotifyOrderStateChanged(this, order);
    }

    protected bool CheckOrders(IEnumerable<IRequestProcessor> requestProcessors, IInboundDataMessage? rm)
    {
        if (rm == null)
        {
            return false;
        }

        foreach (var proc in requestProcessors)
        {
            // Keep request processor local due to asnyc access
            var procLocal = proc;
            var order = procLocal.Order;

            if (procLocal.IsCancelled || order.IsDisposable)
            {
                LogDebug($"message received: order {order?.Id} skipped due to disposed or cancelled");
                continue;
            }

            LogDebug($"message received: check order {order.Id}");

            var success = procLocal.CheckReceivedMessage(rm);
            if (!success)
            {
                Debug.Print($"{rm.ToShortInfoString()} processed unsuccessfully with order {order.Id}.");
                continue;
            }

            // Wait until the order is finished
            Wait.Until(() => order.IsFinished);

            // Now log that
            var msg = $"{rm.ToShortInfoString()} processed successfully with order {order.Id}.";
            LogInformation(msg);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if an order is to be rejected from adding to order queue
    /// </summary>
    /// <param name="order">Order to check</param>
    /// <returns>True if the order has to be rejected (not processed) else false</returns>
    private bool CheckOrder(IOrder order)
    {
        //if (order.deviceId != _smddevice.Id)
        //{
        //    return false;
        //}

        // Check request specs here
        if (order.RequestSpecs.Any(x => x.DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue))
        {
            var orders = AllOrders;
            CheckRequestSpecs(order, orders);
        }

        // Do not add dummy orders
        if (order.TypeName == BuiltinOrders.DummyOrder)
        {
            return false;
        }

        var ps = order.ParameterSet;
        return ps?.IsValid.Count != 0;
    }



    protected void LogDebug(string message)
    {
        MonitorLogger.LogInformation(message);
        AppLogger.LogDebug($"{LoggerId}{message}");
        Debug.Print($"OP: {message}");
    }

    protected void LogInformation(string message)
    {
        MonitorLogger.LogInformation(message);
        AppLogger.LogInformation($"{LoggerId}{message}");
        Debug.Print($"OP: {message}");
    }

    //protected void LogWarning(string message)
    //{
    //    _smddevice.MonitorLogger.LogWarning(message);
    //    _appLogger.LogWarning($"{_loggerId}{message}");
    //    Debug.Print($"TOP: {message}");
    //}

    /// <summary>
    /// Call back method fired from request processor
    /// </summary>
    /// <param name="orderId">Order ID</param>
    protected virtual void OrderProcessingFinished(long orderId)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    protected void StopExecutionOfSyncOrder(long orderId, IOrderExecutionResultState erg)
    {
        var syncData = SyncOrderManager.GetSyncExecutionDataForOrder(orderId);

        if (syncData == null)
        {
            return;
        }

        syncData.TaskCompletionSource?.SetResult(erg);
    }

    protected bool IsCancellationRunningOrders
    {
        get
        {
            lock (IsCancellationRunningOrdersLockObject)
            {
                return IsCancellationRunningOrdersInternal;
            }
        }
        set
        {
            lock (IsCancellationRunningOrdersLockObject)
            {

                IsCancellationRunningOrdersInternal = value;
            }
        }
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseOrderProcessor(
        IOrderManagementDevice deviceServer,
        IAppDateService dateTimeService,
        IOrderPipeline orderPipeline,
        ISyncOrderManager syncOrderManager,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppBenchProxy appBenchProxy)
    {
        CurrentDevice = deviceServer;
        OrderPipeline = orderPipeline;
        SyncOrderManager = syncOrderManager;

        WatchDogRunnerDelegate runner = Runner;
        _messageProcessingWatchdog = new WatchDog(runner, 300);
        ClientNotificationManager = clientNotificationManager;
        DateTimeService = dateTimeService;
        AppLogger = deviceServer.DataMessagingConfig.AppLogger;
        LoggerId = deviceServer.DataMessagingConfig.LoggerId;
        MonitorLogger = deviceServer.DataMessagingConfig.MonitorLogger;
        AppBenchProxy = appBenchProxy;
    }

    /// <summary>
    /// Current order management messing instance
    /// </summary>
    public IOrderManagementClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current order pipeline
    /// </summary>
    public IOrderPipeline OrderPipeline { get; }

    /// <summary>
    /// Current sync order manager
    /// </summary>
    public ISyncOrderManager SyncOrderManager { get; }

    /// <summary>
    /// Current device server instance
    /// </summary>
    public IOrderManagementDevice CurrentDevice { get; }

    /// <summary>
    /// Is no hardware init required on a hardware error. Only for testing purposes. Do NOT integrate in interface
    /// </summary>
    public bool IsNoHardWareInitRequired { get; set; }

    /// <summary>
    /// Executes the waiting orders. Method made public for easier unit testing.
    /// Do not use directly in production code!!!
    /// </summary>
    public virtual void Runner()
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Is the currently runner stopped? 
    /// </summary>
    public bool IsRunnerStopped
    {
        get
        {
            lock (IsRunnerStoppedLock)
            {
                return IsRunnerStoppedInternal;
            }
        }
        set
        {
            bool stopped;
            lock (IsRunnerStoppedLock)
            {
                stopped = IsRunnerStoppedInternal;
                IsRunnerStoppedInternal = value;
            }

            if (!stopped && value)
            {
                AppLogger.LogInformation($"{LoggerId}runner is deactivated now");
            }
            //else
            //{
            //    _appLogger.LogInformation($"{_deviceServer.Smddevice.LoggerId}runner is activated now");
            //}
        }
    }

    /// <summary>
    /// Get all orders in the waiting non-priority queue
    /// </summary>
    public IList<IOrder> OrdersInQueue
    {
        get
        {
            var orders = OrderPipeline.WaitingOrders;
            return orders;
        }
    }

    /// <summary>
    /// Get all waiting but not cancelled orders in the priority queue
    /// </summary>
    public IList<IOrder> OrdersInQueueWithPriority
    {
        get
        {
            var orders = OrderPipeline.WaitingPriorityOrders;

            return orders;
        }
    }

    /// <summary>
    /// Get all orders currently in processing
    /// </summary>
    public IList<IOrder> OrdersInProcessing => OrderPipeline.RunningOrders;

    /// <summary>
    /// Get all orders currently in processing, in the queue and in priority line
    /// </summary>
    public IList<IOrder> AllOrders
    {
        get
        {
            var orders = new List<IOrder>();
            orders.AddRange(OrderPipeline.RunningOrders);
            orders.AddRange(OrderPipeline.WaitingOrders);
            orders.AddRange(OrderPipeline.WaitingPriorityOrders);
            return orders;
        }
    }

    /// <summary>
    /// No order in processing
    /// </summary>
    public bool IsNoOrderInProcessing => OrderPipeline.IsRunningOrdersEmpty;

    /// <summary>
    /// Is only one order in processing?
    /// </summary>
    public bool IsUniqueOrderInProcessing => OrderPipeline.RunningOrdersCount == 1;

    /// <summary>
    /// Is any order in queue to process
    /// </summary>
    public bool IsAnyOrderToProcess
    {
        get
        {
            if (!OrderPipeline.IsRunningOrdersEmpty)
            {
                return true;
            }

            return !OrderPipeline.IsWaitingOrdersEmpty ||
                   !OrderPipeline.IsWaitingPriorityOrdersEmpty;
        }
    }

    /// <summary>
    /// Is there an order running or in the queue breaking a device properties update
    /// </summary>
    public bool IsAnyOrderBreakingdevicePropertiesUpdateToProcess
    {
        get
        {
            if (OrderPipeline.RunningOrders.Any(x => x.IsCancelledOndevicePropertyUpdate))
            {
                return true;
            }
            return OrderPipeline.WaitingOrders.Any(x => x.IsCancelledOndevicePropertyUpdate) ||
                   OrderPipeline.WaitingPriorityOrders.Any(x => x.IsCancelledOndevicePropertyUpdate);
        }
    }
    public long CurrentOrderExecutionDuration { get; set; }

    /// <summary>
    /// Number of orders in processing
    /// </summary>
    /// <remarks>ExecutionQueue.Skip(0).Count() does not require a slow internal lock other than ExecutionQueue.Count() or IsRunningOrdersEmpty</remarks>
    public int OrdersInProcessingCount => OrderPipeline.RunningOrdersCount;

    /// <summary>
    /// Is a StSys init order in the queue or just executing
    /// </summary>
    public bool InitInTheQueue
    {
        get
        {
            if (OrderPipeline.RunningOrdersCount == 0)
            {
                return false;
            }

            var wo = OrderPipeline.WaitingOrders;

            if (wo.Count == 0)
            {
                return false;
            }
            // ToDo: Implement anything for init orders
            return false;

            //return OrderPipeline.RunningOrders.Any(order =>
            //           order.OrderTypeCode is OrderTypeCodes.Init
            //               or OrderTypeCodes.CheckUpdateMode) ||
            //       OrderPipeline.WaitingOrders.Any(order =>
            //           order.OrderTypeCode is OrderTypeCodes.Init
            //               or OrderTypeCodes.CheckUpdateMode);
        }
    }

    /// <summary>
    /// Is an init process processing. True while the orders of StSys init and hardware init are handled. Important to NOT start real order processing in <see cref="Runner "/> before checkslot was handled
    /// </summary>
    public bool IsInitInProcessing
    {
        get
        {
            lock (InitIsProcessingLock)
            {
                return InitIsProcessing;
            }
        }
        set
        {
            lock (InitIsProcessingLock)
            {
                InitIsProcessing = value;
            }

            AppLogger.LogDebug($"{LoggerId}: IsInitInProcessing: {InitIsProcessing}");
        }
    }

    /// <summary>
    /// Is a certain order type running or waiting for execution
    /// </summary>
    /// <param name="orderTypeCode">Order type code</param>
    /// <returns>True if an order is running or waiting in the queue else false</returns>
    public bool IsOrderTypeInTheQueue(int orderTypeCode)
    {
        return OrderPipeline.IsOrderTypeInTheQueue(orderTypeCode);
    }

    /// <summary>
    /// Handle a received error message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public virtual void HandleError(IInboundDataMessage receivedMessage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initiate a hardware init and run the order directly
    /// </summary>
    public IOrder? InitiateHardwareInit()
    {
        return InitiateHardwareInit(true);
    }

    /// <summary>
    /// Initiate a hardware init
    /// </summary>
    /// <param name="runTheOrder">Run the order directly: yes or no</param>
    /// <returns>device hardware init order</returns>
    public virtual IOrder? InitiateHardwareInit(bool runTheOrder)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Cancel all queued and currently running orders
    /// </summary>
    public void CancelAllOrders()
    {
        try
        {
            OrderPipeline.CancelWaitingPriorityOrders();

            OrderPipeline.CancelWaitingOrders();

            CancelRunningOrders(174);
        }
        catch (Exception e)
        {
            AppLogger.LogError("cancelling order failed", e);
        }
    }

    /// <summary>
    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
    /// </summary>
    public void CancelWaitingOrdersOnComDevClose()
    {
        OrderPipeline.CancelWaitingOrdersOnComDevClose();
    }

    /// <summary>
    /// Try to execute an order directly. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <returns>Order execution result</returns>
    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order)
    {
        return TryToExecuteOrderSync(order, false, false);
    }

    /// <summary>
    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
    /// <returns>Order execution result</returns>
    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder)
    {
        return TryToExecuteOrderSync(order, doNotDisposeOrder, false);
    }

    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder, bool directRunOrder)
    {
        var orderId = order.Id;

        order.IsRunningSync = true;

        if (directRunOrder)
        {
            SimpleCheckAndRunOrder(order);
        }
        else
        {
            if (order.IsHighPriorityOrder)
            {
                AddOrderWithPriority(order);
            }
            else
            {
                AddOrder(order);
            }
        }

        var timeout = GetOrderTimeout(order);

        //#if DEBUG
        //            timeout = 5000;
        //#endif

        var syncData = SyncOrderManager.AddSyncExecutionOrder(orderId, timeout);

        // Now wait for order execution (doing it in a non-blocking mannor)
        var erg = AsyncHelper.RunSync(syncData.CreateWaitingTask);

        // Remove the order from waiting queue
        SyncOrderManager.RemoveSyncExecutionOrder(orderId);

        // If the order is still running but sync execution has timed out, cancel order
        if (OrderPipeline.CheckIfOrderIsRunning(orderId))
        {
            OrderPipeline.CancelOrder(orderId);
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId}order has been cancelled");
        }
        else
        {
            AppLogger.LogDebug($"{LoggerId}{order.LoggerId}order has been finished {erg}");
        }

        // Dispose the order now if needed
        if (!doNotDisposeOrder && order.IsDisposable)
        {
            order.Dispose();
        }

        return erg;
    }

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    public void CancelOrder(IOrder order)
    {
        if (order == null)
        {
            return;
        }

        OrderPipeline.CancelOrder(order);
    }

    /// <summary>
    /// Cancel an order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    public void CancelOrder(long orderId)
    {
        if (orderId == 0)
        {
            return;
        }

        OrderPipeline.CancelOrder(orderId);
    }

    /// <summary>
    /// Cancel all orders bound to a certain source item like a joblist
    /// </summary>
    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
    public void CancelOrderBySourceUid(Guid sourceUid)
    {
        OrderPipeline.CancelOrderBySourceUid(sourceUid);
    }

    /// <summary>
    /// Check and then run the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void CheckAndRunOrder(IOrder order)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Check only if parallel running is allowed and if yes run the order
    /// </summary>
    /// <param name="order"></param>
    public void SimpleCheckAndRunOrder(IOrder order)
    {

        if (IsInitInProcessing)
        {
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId}: init is processing, order is cancelled");
            order.IsCancelled = true;
            return;
        }

        AppLogger.LogInformation($"{LoggerId}{order.LoggerId}: check parallel running orders");

        var runningOrders = OrderPipeline.RunningOrders.ToList();

        // Check if the type of order is allowed to run parallel to the current orders
        foreach (var rOrder in runningOrders)
        {
            if (rOrder == null)
            {
                continue;
            }

            var pOrders = rOrder.AllowedParallelOrderTypes;

            // If the new order is allowed to run at the moment leave here
            if (pOrders.Contains(order.TypeName))
            {
                continue;
            }

            AppLogger.LogInformation($"{LoggerId}{order.LoggerId} NOT allowed to run parallel with order {rOrder.LoggerId}");
            order.IsCancelled = true;
            return;
        }

        // Now finally run the order
        RunOrder(order);
    }

    /// <summary>
    /// Run an order directly without checks
    /// </summary>
    /// <param name="order">Order to run without checks</param>
    public void RunOrder(IOrder order)
    {
        // create the processor for the order now
        if (OrderPipeline.PrepareOrderStart(order, OrderProcessingFinished, out var p))
        {
            return;
        }

        // Start benchmarking if necessary
        if (order.IsBenchable)
        {
            order.Benchmark = new Bench(AppBenchProxy, $"{order.DeviceId}_{order.LoggerId}");
        }

        // Execute the order
        order.Benchmark?.AddStep("Before execute");
        ExecuteOrder(p);
        order.Benchmark?.AddStep("After execute");
    }

    /// <summary>
    /// Get the current order processing
    /// </summary>
    /// <returns>Currently processed order</returns>
    public IOrder? GetCurrentProcessingOrder()
    {
        // The former implementation using => produced a null exception sometimes

        if (OrderPipeline.IsRunningOrdersEmpty)
        {
            return null;
        }

        var o = OrderPipeline.RunningOrders.OrderBy(x => x.Id).FirstOrDefault();

        return o;
    }

    /// <summary>
    /// Get the request processor
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    /// <returns>Request processor</returns>
    public IRequestProcessor? GetRequestProcessorForOrder(long orderId)
    {
        foreach (var rp in OrderPipeline.RunningRequestProcessors)
        {
            if (rp.Order.Id == orderId)
            {
                return rp;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds an order to the order processing queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the order queue</param>
    public void AddOrder(IOrder order)
    {
        if (order.IsHighPriorityOrder)
        {
            order.IsHighPriorityOrder = false;
        }

        var json = JsonHelper.JsonSerialize(order.ParameterSet);

        if (CheckOrder(order))
        {
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId}NOT added to queue: {json}");
            return;
        }

        if (OrderPipeline.AllWaitingOrders.Contains(order))
        {
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId}NOT added to queue: order is already existing");
            return;
        }

        OrderPipeline.AddOrder(order);
        AppLogger.LogInformation($"{LoggerId}{order.LoggerId} added to queue. {OrderPipeline.CurrentOrderState}. {json}");
        if (order.IsClientNotificationTurnedOff)
        {
            return;
        }

        Debug.Print($"AddOrder: {OrderPipeline.CurrentOrderState}");
        ClientNotificationManager?.DoNotifyOrderStateChanged(this, order);
    }


    /// <summary>
    /// Adds an order to the order with high priority processing queue for further processing
    /// this should only be used for unload orders after hardware error
    /// </summary>
    /// <param name="order"></param>
    public void AddOrderWithPriority(IOrder order)
    {
        if (!order.IsHighPriorityOrder)
        {
            order.IsHighPriorityOrder = true;
        }

        var json = JsonHelper.JsonSerialize(order.ParameterSet);

        if (CheckOrder(order))
        {
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId} NOT added to priority queue. {json}");
        }
        else
        {
            OrderPipeline.AddPriorityOrder(order);
            ClientNotificationManager?.DoNotifyOrderStateChanged(this, order);
            AppLogger.LogInformation($"{LoggerId}{order.LoggerId} added to priority queue. {OrderPipeline.CurrentOrderState}. {json}");
        }
    }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public virtual bool CheckReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    public void StartOrderProcessing()
    {
        _messageProcessingWatchdog.StartWatchDog();
    }

    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    public void StopOrderProcessing()
    {
        _messageProcessingWatchdog.StopWatchDog();

        IsRunnerStopped = true;

        // ExecutionQueue.OrderBy(x => x.Key)
        foreach (var requestProcessor in OrderPipeline.RunningRequestProcessors)
        {
            if (requestProcessor.IsCancelled)
            {
                requestProcessor.CancellationTokenSource?.Cancel();
                continue;
            }
            requestProcessor.CurrentTask?.Wait(5000);
        }

        IsInitInProcessing = true;

    }

    /// <summary>
    /// Cancel all running orders and run a device order init if required
    /// </summary>
    /// <param name="errorCode">Provided error code from device</param>
    public void CancelRunningOrders(byte errorCode)
    {

        if (IsCancellationRunningOrders || OrderPipeline.IsRunningOrdersEmpty)
        {
            return;
        }

        IsCancellationRunningOrders = true;

        var isRunnerStopped = IsRunnerStopped;
        IsRunnerStopped = true;

        //// Cancel all running orders first
        CurrentDevice.CancelRunningOrders(errorCode);

        var s = $"{LoggerId}running orders cancelled. {OrderPipeline.CurrentOrderState} error code {errorCode}";
        Debug.Print(s);
        AppLogger.LogDebug(s);

        IsCancellationRunningOrders = false;
        IsRunnerStopped = isRunnerStopped;
    }

    /// <summary>
    /// Handle a received error
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public virtual void HandleError(IInboundMessage receivedMessage)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        try
        {
            StopOrderProcessing();
        }
        catch //(Exception e)
        {
            // Do nothing
        }

        OrderPipeline.Dispose();
    }

    /// <summary>
    /// Check if for a new order all request secs are required
    /// </summary>
    /// <param name="order">Order to check</param>
    /// <param name="orders">Orders</param>
    public static void CheckRequestSpecs(IOrder order, IEnumerable<IOrder> orders)
    {

        var oType = order.GetType().Name;

        if (orders.All(x => x.GetType().Name != oType))
        {
            return;
        }

        foreach (var spec in order.RequestSpecs.Where(x => x.DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue).ToList())
        {
            order.RequestSpecs.Remove(spec);
        }
    }

    /// <summary>
    /// Calculate the total order timeout. Public only for unit tests. Do not use directly in code
    /// </summary>
    /// <param name="order">Current order</param>
    /// <returns>Total timeout in ms</returns>
    public static int GetOrderTimeout(IOrder order)
    {
        var timeout = 0;

        try
        {
            foreach (var spec in order.RequestSpecs)
            {
                if (spec.Timeout == int.MaxValue)
                {
                    timeout = 614800; // 7 days + 10000
                    break;
                }

                timeout += spec.Timeout;
            }
        }
        catch
        {
            timeout = 5000;
        }


        if (timeout < 0)
        {
            timeout = 5000;
        }

        // Longest timeout: 7 days (+ 10000)
        if (timeout > 604800)
        {
            timeout = 614800;
        }
        else
        {
            timeout += 10000;
        }
        return timeout;
    }
}