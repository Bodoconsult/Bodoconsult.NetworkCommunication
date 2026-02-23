//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using System.Diagnostics;

//namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

///// <summary>
///// Current implementation of <see cref="IOrderProcessor"/>. This implementation uses a polling mechanism to execute orders instead of producer-consumer-pattern. The reason is that multiple queues have to be handled
///// </summary>
//public class OrderProcessor : IOrderProcessor
//{
//    // Check if cancellation of running orders is already running to avoid endless loop with stack exception
//    private bool _isCancellationRunningOrders;
//    private readonly object _isCancellationRunningOrdersLockObject = new();
        
//    /// <summary>
//    /// Is the runner stopped. DO NOT use this field internally. Use IsRunnerStopped property instead
//    /// </summary>
//    private bool _isRunnerStopped;
//    private readonly object _isRunnerStoppedLock = new();

//    /// <summary>
//    /// true while the orders of StSys init are handled. Important to NOT start real order processing in <see cref="Runner "/> before checkslot was handled
//    /// </summary>
//    private bool _initIsProcessing;
//    private readonly object _initIsProcessingLock = new();

//    /// <summary>
//    /// Watchdog to process messages received from device
//    /// </summary>
//    private readonly IWatchDog _messageProcessingWatchdog;

//    private readonly ISmddeviceConfigAndValues _smddevice;
//    private readonly IDateTimeService _dateTimeService;
//    private readonly IAppLoggerProxy _appLogger;

//    private bool IsCancellationRunningOrders
//    {
//        get
//        {
//            lock (_isCancellationRunningOrdersLockObject)
//            {
//                return _isCancellationRunningOrders;
//            }
//        }
//        set
//        {
//            lock (_isCancellationRunningOrdersLockObject)
//            {

//                _isCancellationRunningOrders = value;
//            }
//        }
//    }

//    /// <summary>
//    /// Default ctor
//    /// </summary>
//    public OrderProcessor(
//        IdeviceServer deviceServer,
//        IDateTimeService dateTimeService,
//        IOrderPipeline orderPipeline,
//        ISyncOrderManager syncOrderManager,
//        IOrderManagementMessaging messagingBusinessDelegate)
//    {
//        CurrentDevice = deviceServer;
//        _smddevice = deviceServer.Smddevice;
//        OrderPipeline = orderPipeline;
//        SyncOrderManager = syncOrderManager;

//        WatchDogRunnerDelegate runner = Runner;
//        _messageProcessingWatchdog = new WatchDog(runner, 300);
//        OrderManagementMessaging = messagingBusinessDelegate;
//        _dateTimeService = dateTimeService;
//        _appLogger = deviceServer.Smddevice.AppLogger;
//    }

//    /// <summary>
//    /// Current order management messing instance
//    /// </summary>
//    public IOrderManagementMessaging OrderManagementMessaging { get; }

//    /// <summary>
//    /// Current order pipeline
//    /// </summary>
//    public IOrderPipeline OrderPipeline { get; }

//    /// <summary>
//    /// Current sync order manager
//    /// </summary>
//    public ISyncOrderManager SyncOrderManager { get; }

//    /// <summary>
//    /// Current device server instance
//    /// </summary>
//    public IdeviceServer CurrentDevice { get; }

//    /// <summary>
//    /// Is the currently runner stopped? 
//    /// </summary>
//    public bool IsRunnerStopped
//    {
//        get
//        {
//            lock (_isRunnerStoppedLock)
//            {
//                return _isRunnerStopped;
//            }
//        }
//        set
//        {
//            bool stopped;
//            lock (_isRunnerStoppedLock)
//            {
//                stopped = _isRunnerStopped;
//                _isRunnerStopped = value;
//            }

//            if (!stopped && value)
//            {
//                _appLogger.LogInformation($"{_smddevice.LoggerId}runner is deactivated now");
//            }
//            //else
//            //{
//            //    _appLogger.LogInformation($"{_deviceServer.Smddevice.LoggerId}runner is activated now");
//            //}
//        }
//    }

//    /// <summary>
//    /// Is no hardware init required on a hardware error. Only for testing purposes. Do NOT integrate in interface
//    /// </summary>
//    public bool IsNoHardWareInitRequired { get; set; }

//    /// <summary>
//    /// Is a certain order type running or waiting for execution
//    /// </summary>
//    /// <param name="orderTypeCode">Order type code: see <see cref="OrderTypeCodes"/> for valid values</param>
//    /// <returns>True if an order is running or waiting in the queue else false</returns>
//    public bool IsOrderTypeInTheQueue(int orderTypeCode)
//    {
//        return OrderPipeline.IsOrderTypeInTheQueue(orderTypeCode);
//    }


//    /// <summary>
//    /// Initiate a hardware init and run the order directly
//    /// </summary>
//    public IOrder InitiateHardwareInit()
//    {
//        return InitiateHardwareInit(true);
//    }

//    /// <summary>
//    /// Initiate a hardware init
//    /// </summary>
//    /// <param name="runTheOrder">Run the order directly: yes or no</param>
//    /// <returns>device hardware init order</returns>
//    public IOrder InitiateHardwareInit(bool runTheOrder)
//    {
//        // Wait until all running orders are ready for disposing finally
//        var orders = OrdersInProcessing;

//        if (orders.Any())
//        {
//            using (var cts = new CancellationTokenSource(5000))
//            {
//                while (!cts.IsCancellationRequested)
//                {
//                    if (orders.All(x => x.IsDisposable))
//                    {
//                        break;
//                    }

//                    Thread.Sleep(5);
//                }
//            }
//        }

//        _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}execution queue cleared for device hardware init");

//        // Is no hardware init required set externally for unit testing
//        if (IsNoHardWareInitRequired)
//        {
//            _smddevice.AppLogger.LogDebug($"{_smddevice.LoggerId}runner restarted");
//            IsRunnerStopped = false;
//            return null;
//        }

//        // Check if already a hardware init is running
//        foreach (var execOrder in OrdersInProcessing)
//        {
//            if (execOrder.TypeName == nameof(deviceHardwareInitOrder))
//            {
//                return null;
//            }
//        }

//        // Create the device hardware init 
//        var order = CurrentDevice.CreateHardwareInitOrder();

//        IsInitInProcessing = true;
//        IsRunnerStopped = true;

//        CheckAndRunOrder(order);

//        CurrentDevice.DoNotifyHardwareInitRequested();
//        _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId}order execution started");

//        return order;
//    }

//    /// <summary>
//    /// Cancel all queued and currently running orders
//    /// </summary>
//    public void CancelAllOrders()
//    {
//        try
//        {
//            OrderPipeline.CancelWaitingPriorityOrders();

//            OrderPipeline.CancelWaitingOrders();

//            CancelRunningOrders(174);

//        }
//        catch (Exception e)
//        {
//            _appLogger.LogError("cancelling order failed", e);
//        }
//    }

//    /// <summary>
//    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
//    /// </summary>
//    public void CancelWaitingOrdersOnComDevClose()
//    {
//        OrderPipeline.CancelWaitingOrdersOnComDevClose();
//    }

//    /// <summary>
//    /// Try to execute an order directly. Depending on order priority the order is loaded to the priority queue or not
//    /// </summary>
//    /// <param name="order">Order to run</param>
//    /// <returns>Order execution result</returns>
//    public OrderExecutionResult TryToExecuteOrderSync(IOrder order)
//    {
//        return TryToExecuteOrderSync(order, false, false);
//    }

//    /// <summary>
//    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
//    /// </summary>
//    /// <param name="order">Order to run</param>
//    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
//    /// <returns>Order execution result</returns>
//    public OrderExecutionResult TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder)
//    {
//        return TryToExecuteOrderSync(order, doNotDisposeOrder, false);
//    }

//    public OrderExecutionResult TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder, bool directRunOrder)
//    {
//        var orderId = order.Id;

//        order.IsRunningSync = true;

//        if (directRunOrder)
//        {
//            SimpleCheckAndRunOrder(order);
//        }
//        else
//        {
//            if (order.IsHighPriorityOrder)
//            {
//                AddOrderWithPriority(order);
//            }
//            else
//            {
//                AddOrder(order);
//            }
//        }

//        var timeout = GetOrderTimeout(order);

////#if DEBUG
////            timeout = 5000;
////#endif

//        var syncData = SyncOrderManager.AddSyncExecutionOrder(orderId, timeout);

//        // Now wait for order execution (doing it in a non-blocking mannor)
//        var erg = AsyncHelper.RunSync(syncData.CreateWaitingTask);

//        // Remove the order from waiting queue
//        SyncOrderManager.RemoveSyncExecutionOrder(orderId);

//        // If the order is still running but sync execution has timed out, cancel order
//        if (OrderPipeline.CheckIfOrderIsRunning(orderId))
//        {
//            OrderPipeline.CancelOrder(orderId);
//            _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId}order has been cancelled");
//        }
//        else
//        {
//            _smddevice.AppLogger.LogDebug($"{_smddevice.LoggerId}{order.LoggerId}order has been finished {erg.LastStepExecutionResult}");
//        }

//        // Dispose the order now if needed
//        if (!doNotDisposeOrder && order.IsDisposable)
//        {
//            order.Dispose();
//        }

//        return erg;
//    }


//    /// <summary>
//    /// Calculate the total order timeout. Public only for unit tests. Do not use directly in code
//    /// </summary>
//    /// <param name="order">Current order</param>
//    /// <returns>Total timeout in ms</returns>
//    public static int GetOrderTimeout(IOrder order)
//    {
//        var timeout = 0;

//        try
//        {
//            foreach (var spec in order.RequestSpecs)
//            {
//                if (spec.Timeout == int.MaxValue)
//                {
//                    timeout = 614800; // 7 days + 10000
//                    break;
//                }

//                timeout += spec.Timeout;
//            }
//        }
//        catch
//        {
//            timeout = 5000;
//        }


//        if (timeout < 0)
//        {
//            timeout = 5000;
//        }

//        // Longest timeout: 7 days (+ 10000)
//        if (timeout > 604800)
//        {
//            timeout = 614800;
//        }
//        else
//        {
//            timeout += 10000;
//        }
//        return timeout;
//    }

//    /// <summary>
//    /// Cancel a running order
//    /// </summary>
//    /// <param name="order">Order to cancel</param>
//    public void CancelOrder(IOrder order)
//    {
//        if (order == null)
//        {
//            return;
//        }

//        OrderPipeline.CancelOrder(order);
//    }

//    /// <summary>
//    /// Cancel a order via its ID
//    /// </summary>
//    /// <param name="orderId">ID of the order to cancel</param>
//    public void CancelOrder(long orderId)
//    {
//        if (orderId == 0)
//        {
//            return;
//        }

//        OrderPipeline.CancelOrder(orderId);
//    }

//    /// <summary>
//    /// Cancel all orders bound to a certain source item like a joblist
//    /// </summary>
//    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
//    public void CancelOrderBySourceUid(Guid sourceUid)
//    {
//        OrderPipeline.CancelOrderBySourceUid(sourceUid);
//    }


//    /// <summary>
//    /// Executes the waiting orders. Method made public for easier unit testing.
//    /// Do not use directly in production code!!!
//    /// See https://confluence.miclaser.net/display/SMDT/TOM%3A+device+order+processor+order+work+flows for details
//    /// </summary>
//    public void Runner()
//    {
//        if (!OrderPipeline.IsRunningOrdersEmpty)
//        {
//            OrderPipeline.RemoveCancelledOrdersFromExecutionQueue();
//        }

//        // Check conditions in order processor if running an order is allowed
//        var isNoOrderWaiting = (OrderPipeline.IsWaitingOrdersEmpty && OrderPipeline.IsWaitingPriorityOrdersEmpty);
//        if (IsRunnerStopped || isNoOrderWaiting)
//        {
//            if (isNoOrderWaiting && !IsInitInProcessing)
//            {
//                CurrentDevice.CheckIfThereAreOrdersToBeCreated();
//            }
//            return;
//        }

//        // Check if the device is ready for order running
//        if (!CurrentDevice.IsRunningOrdersAllowed)
//        {
//            return;
//        }

//        // Get the next order to run from business logic
//        var order = CurrentDevice.GetNextOrderToRun();

//        if (order == null)
//        {
//            return;
//        }

//        // Run the order now
//        CheckAndRunOrder(order);

//    }

//    /// <summary>
//    /// Check and then run the order
//    /// </summary>
//    /// <param name="order">Current order</param>
//    public void CheckAndRunOrder(IOrder order)
//    {
//        var runningOrders = OrderPipeline.RunningOrders.ToList();

//        // Check if it is allowed to run the order now
//        if (!CurrentDevice.IsRunningTheOrderAllowed(order, runningOrders))
//        {
//            return;
//        }

//        // Dequeue the order now go get it out pipeline
//        if (OrderPipeline.DequeueOrder(order))
//        {
//            return;
//        }

//        // last check to prevent from deadlock
//        CurrentDevice.Check4ConcurrentOrders(order);

//        // Now finally execute the order
//        RunOrder(order);
//    }

//    /// <summary>
//    /// Check only if parallel running is allowed and if yes run the order
//    /// </summary>
//    /// <param name="order"></param>
//    public void SimpleCheckAndRunOrder(IOrder order)
//    {

//        if (IsInitInProcessing)
//        {
//            _appLogger.LogInformation($"{_smddevice.LoggerId }{order.LoggerId}: init is processing, order is cancelled");
//            order.IsCancelled = true;
//            return;
//        }

//        _appLogger.LogInformation($"{_smddevice.LoggerId }{order.LoggerId}: check parallel running orders");

//        var runningOrders = OrderPipeline.RunningOrders.ToList();

//        // Check if the type of order is allowed to run parallel to the current orders
//        foreach (var rOrder in runningOrders)
//        {
//            if (rOrder == null)
//            {
//                continue;
//            }

//            var pOrders = rOrder.AllowedParallelOrderTypes;

//            // If the new order is allowed to run at the moment leave here
//            if (pOrders.Contains(order.TypeName))
//            {
//                continue;
//            }

//            _appLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId} NOT allowed to run parallel with order {rOrder.LoggerId}");
//            order.IsCancelled = true;
//            return;
//        }

//        // Now finally run the order
//        RunOrder(order);
//    }

//    /// <summary>
//    /// Run an order directly without checks
//    /// </summary>
//    /// <param name="order">Order to run without checks</param>
//    public void RunOrder(IOrder order)
//    {
//        // create the processor for the order now
//        if (OrderPipeline.PrepareOrderStart(order, OrderProcessingFinished, out var p))
//        {
//            return;
//        }

//        // Execute the order
//        order.Benchmark.AddStep("Before execute");
//        ExecuteOrder(p);
//        order.Benchmark.AddStep("After execute");
//    }

//    /// <summary>
//    /// Start the request processor to execute the order
//    /// </summary>
//    /// <param name="requestProcessor">Current request processor</param>
//    /// <returns>Task with order execution</returns>
//    private void ExecuteOrder(IRequestProcessor requestProcessor)
//    {
//        var order = requestProcessor.Order;
//        if (order == null)
//        {
//            return;
//        }

//        // Now execute
//        OrderPipeline.ExecuteOrder(requestProcessor);

//        // Notify the app on order execution?
//        if (order.IsClientNotificationTurnedOff)
//        {
//            // No notification
//            return;
//        }

//        // Notify async
//        OrderManagementMessaging.DoNotifyOrderStateChanged(this, order);
//    }

//    /// <summary>
//    /// Call back method fired from request processor
//    /// </summary>
//    /// <param name="orderId"></param>
//    private void OrderProcessingFinished(long orderId)
//    {
//        var requestProcessor = OrderPipeline.GetFromExecutionQueue(orderId);

//        if (requestProcessor == null)
//        {
//            var erg = new OrderExecutionResult
//            {
//                Finished = true,
//                LastStepExecutionResult = OrderExecutionResultState.Unsuccessful,
//                CustomResultObject = null,
//            };

//            StopExecutionOfSyncOrder(orderId, erg);

//            _smddevice.AppLogger.LogDebug($"{_smddevice.LoggerId}order {orderId} has finished but request processor wasn't found");
//            return;
//        }

//        // Deactivate order handling here
//        requestProcessor.OrderProcessingFinishedDelegate = null;

//        var order = requestProcessor.Order;
//        order.ExecutionResult ??= OrderExecutionResultState.Unsuccessful;
//        order.Benchmark.AddStep("Order processing finished");

//        _smddevice.AppLogger.LogDebug($"{_smddevice.LoggerId}{order.LoggerId}has finished. {OrderPipeline.CurrentOrderState}");

//        // Process successful orders
//        if (order.ExecutionResult.Id == OrderExecutionResultState.Successful.Id)
//        {
//            CurrentDevice.OrderFinishedSuccessful(order);
//            order.Benchmark.AddStep("Order finished Successful");
//        }
//        else // Process unsuccessful orders
//        {
//            CurrentDevice.OrderFinishedUnsuccessful(order);
//            order.Benchmark.AddStep("Order finished Unsuccessful");
//        }

//        // Now clean the execution queue
//        OrderPipeline.RemoveFromExecutionQueue(orderId);
//        requestProcessor.Dispose();

//        // Set the order running endtime
//        order.EndTime = _dateTimeService.Now;
//        order.IsFinished = true;
//        order.Benchmark.AddStep("Disposing now");

//        // Async order: dispose now

//        if (!order.IsRunningSync)
//        {
//            order.Dispose();
//        }
//        else // Sync order: give control back to calling method
//        {
//            // Create the order execution result for a sync running order
//            var erg = new OrderExecutionResult
//            {
//                Finished = true,
//                LastStepExecutionResult = order.ExecutionResult,
//                CustomResultObject = order.ParameterSet.OrderResult,
//            };

//            order.IsDisposable = true;
//            StopExecutionOfSyncOrder(order.Id, erg);

//        }


//        Debug.Print($"TOP: {OrderPipeline.CurrentOrderState}");
//    }

//    private void StopExecutionOfSyncOrder(long orderId, OrderExecutionResult erg)
//    {
//        var syncData = SyncOrderManager.GetSyncExecutionDataForOrder(orderId);
//        syncData?.TaskCompletionSource.SetResult(erg);
//    }


//    /// <summary>
//    /// Get all orders in the waiting non-priority queue
//    /// </summary>
//    public IList<IOrder> OrdersInQueue
//    {
//        get
//        {
//            var orders = OrderPipeline.WaitingOrders;
//            return orders;
//        }
//    }

//    /// <summary>
//    /// Get all waiting but not cancelled orders in the priority queue
//    /// </summary>
//    public IList<IOrder> OrdersInQueueWithPriority
//    {
//        get
//        {
//            var orders = OrderPipeline.WaitingPriorityOrders;

//            return orders;
//        }
//    }

//    /// <summary>
//    /// Get all orders currently in processing
//    /// </summary>
//    public IList<IOrder> OrdersInProcessing => OrderPipeline.RunningOrders;

//    /// <summary>
//    /// Get all orders currently in processing, in the queue and in priority line
//    /// </summary>
//    public IList<IOrder> AllOrders
//    {
//        get
//        {
//            var orders = new List<IOrder>();
//            orders.AddRange(OrderPipeline.RunningOrders);
//            orders.AddRange(OrderPipeline.WaitingOrders);
//            orders.AddRange(OrderPipeline.WaitingPriorityOrders);
//            return orders;
//        }
//    }

//    /// <summary>
//    /// No order in processing
//    /// </summary>
//    public bool IsNoOrderInProcessing => OrderPipeline.IsRunningOrdersEmpty;

//    /// <summary>
//    /// Is only one order in processing?
//    /// </summary>
//    public bool IsUniqueOrderInProcessing => OrderPipeline.RunningOrdersCount == 1;

//    /// <summary>
//    /// Is any order in queue to process
//    /// </summary>
//    public bool IsAnyOrderToProcess
//    {
//        get
//        {
//            if (!OrderPipeline.IsRunningOrdersEmpty)
//            {
//                return true;
//            }

//            return !OrderPipeline.IsWaitingOrdersEmpty ||
//                   !OrderPipeline.IsWaitingPriorityOrdersEmpty;
//        }
//    }

//    /// <summary>
//    /// Is there a order running or in the queue breaking a device properties update
//    /// </summary>
//    public bool IsAnyOrderBreakingdevicePropertiesUpdateToProcess
//    {
//        get
//        {
//            if (OrderPipeline.RunningOrders.Any(x => x.IsCancelledOndevicePropertyUpdate))
//            {
//                return true;
//            }
//            return OrderPipeline.WaitingOrders.Any(x => x.IsCancelledOndevicePropertyUpdate) ||
//                   OrderPipeline.WaitingPriorityOrders.Any(x => x.IsCancelledOndevicePropertyUpdate);
//        }
//    }

//    /// <summary>
//    /// Get the current order processing
//    /// </summary>
//    /// <returns>Currently processed order</returns>
//    public IOrder GetCurrentProcessingOrder()
//    {
//        // The former implementation using => produced a null exception sometimes

//        if (OrderPipeline.IsRunningOrdersEmpty)
//        {
//            return null;
//        }

//        var o = OrderPipeline.RunningOrders.OrderBy(x => x.Id).FirstOrDefault();

//        return o;
//    }

//    /// <summary>
//    /// Get the request processor
//    /// </summary>
//    /// <param name="orderId">Current order ID</param>
//    /// <returns>Request processor</returns>
//    public IRequestProcessor GetRequestProcessorForOrder(long orderId)
//    {
//        foreach (var rp in OrderPipeline.RunningRequestProcessors)
//        {
//            if (rp.Order.Id == orderId)
//            {
//                return rp;
//            }
//        }

//        return null;
//    }

//    public long CurrentOrderExecutionDuration { get; set; }

//    /// <summary>
//    /// Is a previous unload still processing
//    /// </summary>
//    public bool IsPreviousUnloadProcessing => OrderPipeline.RunningOrders.Any(order => order.OrderTypeCode is OrderTypeCodes.Unload
//            or OrderTypeCodes.UnloadPreProcessing
//            or OrderTypeCodes.UnloadPreProcessingMove
//            or OrderTypeCodes.UnloadTrialRun
//        && !order.IsCancelled
//        && !order.IsFinished
//    );

//    /// <summary>
//    /// Number of orders in processing
//    /// </summary>
//    /// <remarks>ExecutionQueue.Skip(0).Count() does not require a slow internal lock other than ExecutionQueue.Count() or IsRunningOrdersEmpty</remarks>
//    public int OrdersInProcessingCount => OrderPipeline.RunningOrdersCount;

//    /// <summary>
//    /// Is a StSys init order in the queue or just executing
//    /// </summary>
//    public bool InitInTheQueue
//    {
//        get
//        {
//            if (OrderPipeline.RunningOrdersCount == 0)
//            {
//                return false;
//            }

//            var wo = OrderPipeline.WaitingOrders;

//            if (wo.Count == 0)
//            {
//                return false;
//            }

//            return OrderPipeline.RunningOrders.Any(order =>
//                       order.OrderTypeCode is OrderTypeCodes.Init
//                           or OrderTypeCodes.CheckUpdateMode) ||
//                   OrderPipeline.WaitingOrders.Any(order =>
//                       order.OrderTypeCode is OrderTypeCodes.Init
//                           or OrderTypeCodes.CheckUpdateMode);
//        }
//    }



//    /// <summary>
//    /// Is an init process processing. True while the orders of StSys init and hardware init are handled. Important to NOT start real order processing in <see cref="Runner "/> before checkslot was handled
//    /// </summary>
//    public bool IsInitInProcessing
//    {
//        get
//        {
//            lock (_initIsProcessingLock)
//            {
//                return _initIsProcessing;
//            }
//        }
//        set
//        {
//            lock (_initIsProcessingLock)
//            {
//                _initIsProcessing = value;
//            }

//            _smddevice.AppLogger.LogDebug($"{_smddevice.LoggerId}: IsInitInProcessing: {_initIsProcessing}");
//        }
//    }

//    /// <summary>
//    /// Adds an order to the order processing queue for further processing
//    /// </summary>
//    /// <param name="order">Order to add to the order queue</param>
//    public void AddOrder(IOrder order)
//    {
//        if (order.IsHighPriorityOrder)
//        {
//            order.IsHighPriorityOrder = false;
//        }

//        if (CheckOrder(order))
//        {
//            _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId}NOT added to queue: {JsonHelper.JsonSerialize(order.ParameterSet)}");
//            return;
//        }

//        OrderPipeline.AddOrder(order);

//        _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId} added to queue. {OrderPipeline.CurrentOrderState}. {JsonHelper.JsonSerialize(order.ParameterSet)}");
//        if (order.OrderTypeCode is OrderTypeCodes.RegularState or
//            OrderTypeCodes.Init or
//            OrderTypeCodes.Cancel or
//            OrderTypeCodes.RegularTemperatureHumidity)
//        {
//            return;
//        }

//        Debug.Print($"AddOrder: {OrderPipeline.CurrentOrderState}");
//        OrderManagementMessaging?.DoNotifyOrderStateChanged(this, order);
//    }


//    /// <summary>
//    /// Adds an order to the order with high priority processing queue for further processing
//    /// this should only be used for unload orders after hardware error
//    /// </summary>
//    /// <param name="order"></param>
//    public void AddOrderWithPriority(IOrder order)
//    {
//        if (!order.IsHighPriorityOrder)
//        {
//            order.IsHighPriorityOrder = true;
//        }

//        if (CheckOrder(order))
//        {
//            _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId} NOT added to priority queue. {JsonHelper.JsonSerialize(order.ParameterSet)}");
//        }
//        else
//        {
//            OrderPipeline.AddPriorityOrder(order);
//            OrderManagementMessaging?.DoNotifyOrderStateChanged(this, order);
//            _smddevice.AppLogger.LogInformation($"{_smddevice.LoggerId}{order.LoggerId} added to priority queue. {OrderPipeline.CurrentOrderState}. {JsonHelper.JsonSerialize(order.ParameterSet)}");
//        }
//    }

//    /// <summary>
//    /// Check if a order is to be rejected from adding to order queue
//    /// </summary>
//    /// <param name="order">Order to check</param>
//    /// <returns>True if the order has to be rejected (not processed) else false</returns>
//    private bool CheckOrder(IOrder order)
//    {
//        if (order.deviceId != _smddevice.Id)
//        {
//            return false;
//        }

//        IList<IOrder> orders = null;

//        // Check request specs here
//        if (order.RequestSpecs.Any(x => x.DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue))
//        {
//            orders = AllOrders;
//            CheckRequestSpecs(order, orders);
//        }


//        // Now proceed only in case of unload orders
//        if (order.OrderTypeCode != OrderTypeCodes.Unload)
//        {
//            return false;
//        }

//        // Do not add dummy orders
//        if (order is DummyMaterialUnloadOrder)
//        {
//            return false;
//        }

//        var sParameterSet = (ISDataBlockParameterSet)order.ParameterSet;
//        if (sParameterSet.TestMode)
//        {
//            return false;
//        }

//        // Check if there is already a order for the same carrier
//        var ops = (ISDataBlockParameterSet)order.ParameterSet;

//        orders ??= AllOrders;

//        foreach (var execOrder in orders)
//        {
//            if (execOrder.OrderTypeCode is not OrderTypeCodes.Unload
//                or OrderTypeCodes.UnloadPreProcessing
//                or OrderTypeCodes.UnloadPreProcessingMove
//                or OrderTypeCodes.UnloadTrialRun)
//            {
//                continue;
//            }

//            var ps = (ISDataBlockParameterSet)execOrder.ParameterSet;
//            if (ops.CarrierName.Equals(ps.CarrierName) || ops.CarrierUid == ps.CarrierUid)
//            {
//                return true;
//            }
//        }

//        return false;
//    }

//    /// <summary>
//    /// Check if for a new order all request secs are required
//    /// </summary>
//    /// <param name="order">Order to check</param>
//    /// <param name="orders">Orders</param>
//    public void CheckRequestSpecs(IOrder order, IEnumerable<IOrder> orders)
//    {

//        var oType = order.GetType().Name;

//        if (orders.All(x => x.GetType().Name != oType))
//        {
//            return;
//        }

//        foreach (var spec in order.RequestSpecs.Where(x => x.DoNotRunRequestSpecIfThereIsSameOrderTypeInQueue).ToList())
//        {
//            order.RequestSpecs.Remove(spec);
//        }
//    }

//    /// <summary>
//    /// Check a received message
//    /// </summary>
//    /// <param name="receivedMessage">A message received from the device</param>
//    /// <returns>True if the message was an expected answer of the current request</returns>
//    public bool CheckReceivedMessage(IDataMessage receivedMessage)
//    {
//        if (receivedMessage == null)
//        {
//            return false;
//        }

//        string msg;

//        if (receivedMessage is not IInboundDataMessage rm)
//        {
//            msg = $"received {receivedMessage.ToInfoString()} {OrderPipeline.CurrentOrderState}";
//            LogInformation(msg);
//            return false;
//        }

//        Debug.Print($"TOP: message received: {receivedMessage.ToInfoString()}");

//        //*********************
//        // TOP 1 A X message with a error code of 0 makes no sense: throw this message away
//        // This other X messages should be given to the order pipeline first and after that it should be handled befor given to async message handler
//        //*********************
//        if (CurrentDevice.DoBasicCheckForReceivedMessage(rm))
//        {
//            return true;
//        }

//        msg = $"received {receivedMessage.ToInfoString()} {OrderPipeline.CurrentOrderState}";
//        LogInformation(msg);

//        //*********************
//        // TOP 2 Deliver message to order pipeline with all running orders with higher priority to check
//        //*********************
//        var requestProcessors = OrderPipeline.RunningRequestProcessors;
//        if (CheckOrders(requestProcessors.Where(x => x.Order!=null && x.Order.IsCheckedWithPriority && !x.Order.WasSuccessful  && !x.Order.IsFinished), rm))
//        {
//            return true;
//        }

//        //*********************
//        // TOP 3 Deliver message to order pipeline with all running orders with normal priority to check
//        //*********************
//        if (CheckOrders(requestProcessors.Where(x => x.Order!=null && !x.Order.IsCheckedWithPriority && !x.Order.WasSuccessful  && !x.Order.IsFinished), rm))
//        {
//            return true;
//        }

//        //*********************
//        // TOP 4 Handle an error message now if not handled by any order before
//        //*********************
//        if (CurrentDevice.DoCheckForErrorMessage(rm))
//        {
//            return true;
//        }

//        //*********************
//        // TOP 5 Last chance for message handling: async message handling
//        //*********************
//        var result = CurrentDevice.CurrentState.HandleAsyncMessage(receivedMessage);

//        //IsRunnerStopped = false;

//        if (result == null)
//        {
//            msg = $"{receivedMessage.ToShortInfoString()}: async processed unsuccessful. Message is disposed now";
//            LogInformation(msg);
//            return false;
//        }

//        msg = $"{receivedMessage.ToShortInfoString()}: async processed {result.ExecutionResult}{(result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id ? "" : ". Message is disposed now")}";
//        LogInformation(msg);

//        return result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id;

//    }

//    private bool CheckOrders(IEnumerable<IRequestProcessor> requestProcessors, IInboundDataMessage rm)
//    {
//        foreach (var proc in requestProcessors)
//        {
//            // Keep request processor local due to asnyc access
//            var procLocal = proc;
//            var order = procLocal.Order;

//            if (order == null || procLocal.IsCancelled || order.IsDisposable)
//            {
//                LogDebug($"message received: order {order?.Id} skipped due to disposed or cancelled");
//                continue;
//            }

//            LogDebug($"message received: check order {order.Id}");

//            var success = procLocal.CheckReceivedMessage(rm);
//            if (!success)
//            {
//                Debug.Print($"{rm.ToShortInfoString()} processed unsuccessfully with order {order.Id}.");
//                continue;
//            }


//            // Wait until the order is finished
//            Wait.Until(() => order.IsFinished);

//            // Now log that
//            var msg = $"{rm.ToShortInfoString()} processed successfully with order {order.Id}.";
//            LogInformation(msg);
//            return true;
//        }

//        return false;
//    }

//    private void LogDebug(string message)
//    {
//        _smddevice.MonitorLogger.LogInformation(message);
//        _appLogger.LogDebug($"{_smddevice.LoggerId}{message}");
//        Debug.Print($"TOP: {message}");
//    }

//    private void LogInformation(string message)
//    {
//        _smddevice.MonitorLogger.LogInformation(message);
//        _appLogger.LogInformation($"{_smddevice.LoggerId}{message}");
//        Debug.Print($"TOP: {message}");
//    }

//    //private void LogWarning(string message)
//    //{
//    //    _smddevice.MonitorLogger.LogWarning(message);
//    //    _appLogger.LogWarning($"{_smddevice.LoggerId}{message}");
//    //    Debug.Print($"TOP: {message}");
//    //}

//    /// <summary>
//    /// Cancel all running orders and run a device order init if required
//    /// </summary>
//    /// <param name="errorCode">Provided error code from device. Hardware errors are error codes greater equal 175 and not 229, 223, 249</param>
//    public void CancelRunningOrders(byte errorCode)
//    {

//        if (IsCancellationRunningOrders || OrderPipeline.IsRunningOrdersEmpty)
//        {
//            return;
//        }

//        IsCancellationRunningOrders = true;

//        var isRunnerStopped = IsRunnerStopped;
//        IsRunnerStopped = true;

//        //// Cancel all running orders first
//        CurrentDevice.CancelRunningOrders(errorCode);

//        var s = $"{_smddevice.LoggerId}running orders cancelled. {OrderPipeline.CurrentOrderState} error code {errorCode}";
//        Debug.Print(s);
//        _appLogger.LogDebug(s);

//        IsCancellationRunningOrders = false;
//        IsRunnerStopped = isRunnerStopped;
//    }


//    /// <summary>
//    /// Handle a received error
//    /// </summary>
//    /// <param name="receivedMessage">Received message</param>
//    public void HandleError(IDataMessage receivedMessage)
//    {
//        if (receivedMessage is not IInboundDataMessage msg)
//        {
//            return;
//        }

//        CurrentDevice.CurrentState.HandleErrorMessage(msg);
//    }

//    /// <summary>
//    /// Starts the watchdog for the order processing
//    /// </summary>
//    public void StartOrderProcessing()
//    {
//        _messageProcessingWatchdog.StartWatchDog();
//    }

//    /// <summary>
//    /// Stops the watchdog for the order processing
//    /// </summary>
//    public void StopOrderProcessing()
//    {
//        _messageProcessingWatchdog.StopWatchDog();

//        IsRunnerStopped = true;

//        // ExecutionQueue.OrderBy(x => x.Key)
//        foreach (var requestProcessor in OrderPipeline.RunningRequestProcessors)
//        {
//            if (requestProcessor.IsCancelled)
//            {
//                requestProcessor.CancellationTokenSource.Cancel();
//                continue;
//            }
//            requestProcessor.CurrentTask?.Wait(5000);
//        }

//        IsInitInProcessing = true;

//    }

//    /// <summary>
//    /// Check if there is an unload order already for the same container. If yes, add the new carrier to this unload
//    /// </summary>
//    /// <param name="containerUid">UID of the container</param>
//    /// <param name="carrierUid">UID of the carrier to unload</param>
//    /// <returns>True if the carrier was added to an existing order else false</returns>
//    public bool Check4UnloadOrdersForSameContainer(Guid containerUid, Guid carrierUid)
//    {
//        var o1 = OrderPipeline.RunningOrders;
//        var o2 = OrderPipeline.WaitingOrders;

//        // Check current order
//        foreach (var order in o1)
//        {

//            var spec = order.RequestSpecs.FirstOrDefault(x => x.Name is nameof(ExpressMaterialUnloadRequestSpec) or nameof(MaterialUnloadRequestSpec));

//            if (order.IsDisposable || order is not IMaterialUnloadOrder unloadOrder || spec == null || spec.RequestAnswerSteps.Count == 0)
//            {
//                continue;
//            }

//            if (unloadOrder.SParameterSet.CarrierUid != containerUid)
//            {
//                continue;
//            }

//            // If the B was received already do not add carrier
//            if (spec.RequestAnswerSteps[^1].WasSuccessful)
//            {
//                continue;
//            }

//            unloadOrder.SParameterSet.RequestedCarrierUids.Add(carrierUid);
//            return true;
//        }

//        // Check for waiting orders
//        foreach (var order in o2)
//        {

//            if (order is not IMaterialUnloadOrder unloadOrder)
//            {
//                continue;
//            }

//            if (unloadOrder.SParameterSet.CarrierUid != containerUid)
//            {
//                continue;
//            }

//            unloadOrder.SParameterSet.RequestedCarrierUids.Add(carrierUid);
//            return true;
//        }

//        return false;
//    }


//    /// <summary>
//    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//    /// </summary>
//    public void Dispose()
//    {
//        try
//        {
//            StopOrderProcessing();
//        }
//        catch //(Exception e)
//        {
//            // Do nothing
//        }

//        OrderPipeline.Dispose();
//    }
//}