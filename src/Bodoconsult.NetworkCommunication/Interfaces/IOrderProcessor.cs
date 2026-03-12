// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// The worker module of the order management: manages order processing on top level
/// </summary>
public interface IOrderProcessor : IDisposable
{
    /// <summary>
    /// Current order management messing instance
    /// </summary>
    IOrderManagementClientNotificationManager ClientNotificationManager { get; }

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
    IOrderManagementDevice CurrentDevice { get; }

    /// <summary>
    /// Is the runner stopped? 
    /// </summary>
    bool IsRunnerStopped { get; set; }

    /// <summary>
    /// Is a certain order type running or waiting for execution
    /// </summary>
    /// <param name="orderTypeCode">Order type code</param>
    /// <returns>True if an order is running or waiting in the queue else false</returns>
    bool IsOrderTypeInTheQueue(int orderTypeCode);

    /// <summary>
    /// Get all orders in the waiting non-priority queue
    /// </summary>
    IList<IOrder> OrdersInQueue { get; }

    /// <summary>
    /// Get all waiting but not cancelled orders in the priority queue
    /// </summary>
    IList<IOrder> OrdersInQueueWithPriority { get; }

    /// <summary>
    /// Get all waiting but not cancelled orders currently in processing
    /// </summary>
    IList<IOrder> OrdersInProcessing { get; }

    /// <summary>
    /// Get all orders currently in processing, in the queue and in priority line
    /// </summary>
    IList<IOrder> AllOrders { get; }

    /// <summary>
    /// No order in processing
    /// </summary>
    bool IsNoOrderInProcessing { get; }

    /// <summary>
    /// Is only one order in processing?
    /// </summary>
    bool IsUniqueOrderInProcessing { get; }

    /// <summary>
    /// Is any order in queue to process
    /// </summary>
    bool IsAnyOrderToProcess { get; }

    /// <summary>
    /// Is there an order running or in the queue breaking a device properties update
    /// </summary>
    bool IsAnyOrderBreakingdevicePropertiesUpdateToProcess { get; }

    /// <summary>
    /// Get the current order processing
    /// </summary>
    /// <returns>Currently processed order</returns>
    IOrder? GetCurrentProcessingOrder();

    /// <summary>
    /// Get the request processor
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    /// <returns>Request processor</returns>
    IRequestProcessor? GetRequestProcessorForOrder(long orderId);

    long CurrentOrderExecutionDuration { get; }

    /// <summary>
    /// Number of orders in processing
    /// </summary>
    int OrdersInProcessingCount { get; }

    /// <summary>
    /// Is a StSys init order in the queue or just executing
    /// </summary>
    bool InitInTheQueue { get; }

    /// <summary>
    /// Is an init process processing. True while the orders of StSys init and hardware init are handled. Important to NOT start real order processing in <see cref="Runner "/> before checkslot was handled
    /// </summary>
    bool IsInitInProcessing { get; set; }

    /// <summary>
    /// Adds an order to the order processing queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the order queue</param>
    void AddOrder(IOrder order);

    /// <summary>
    /// Adds an order to the order with high priority processing queue for further processing
    /// this is meant for unload orders after hardware error
    /// </summary>
    /// <param name="order">Order to add to the order queue</param>
    void AddOrderWithPriority(IOrder order);

    /// <summary>
    /// Cancel all running orders and run a device order init if required
    /// </summary>
    /// <param name="errorCode">Provided error code from device</param>
    void CancelRunningOrders(byte errorCode);

    /// <summary>
    /// Handle a received error message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    void HandleError(IInboundDataMessage receivedMessage);

    /// <summary>
    /// Initiate a hardware init and run the order directly
    /// </summary>
    IOrder? InitiateHardwareInit();

    /// <summary>
    /// Initiate a hardware init
    /// </summary>
    /// <param name="runTheOrder">Run the order directly: yes or no</param>
    /// <returns>device hardware init order</returns>
    IOrder? InitiateHardwareInit(bool runTheOrder);


    /// <summary>
    /// Cancel all queued and currently running orders
    /// </summary>
    void CancelAllOrders();

    /// <summary>
    /// Cancel all queued non-priority orders marked as to be cancelled on ComDevClose
    /// </summary>
    void CancelWaitingOrdersOnComDevClose();

    /// <summary>
    /// Try to execute an order directly. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <returns>Order execution result</returns>
    IOrderExecutionResultState TryToExecuteOrderSync(IOrder order);

    /// <summary>
    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
    /// <returns>Order execution result</returns>
    IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder);

    /// <summary>
    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
    /// <param name="directRunOrder">Run the roder directly without using queue</param>
    /// <returns>Order execution result</returns>
    IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder, bool directRunOrder);

    /// <summary>
    /// Cancel a running order
    /// </summary>
    /// <param name="order">Order to cancel</param>
    void CancelOrder(IOrder order);

    /// <summary>
    /// Cancel an order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    void CancelOrder(long orderId);

    /// <summary>
    /// Cancel all orders bound to a certain source item like a joblist
    /// </summary>
    /// <param name="sourceUid">The UID of a source item like a joblist the order is bound to</param>
    void CancelOrderBySourceUid(Guid sourceUid);


    /// <summary>
    /// Executes the waiting orders. Method made public for easier unit testing.
    /// Do not use directly in production code!!!
    /// </summary>
    void Runner();

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    bool CheckReceivedMessage(IInboundDataMessage? receivedMessage);


    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    void StartOrderProcessing();


    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    void StopOrderProcessing();
}