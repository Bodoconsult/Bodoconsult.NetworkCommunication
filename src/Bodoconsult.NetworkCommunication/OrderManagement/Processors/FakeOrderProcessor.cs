// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Fake implementation for <see cref="IOrderProcessor"/>
/// </summary>
public class FakeOrderProcessor : IOrderProcessor
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public FakeOrderProcessor(
        IOrderManagementDevice device,
        IOrderPipeline orderPipeline,
        ISyncOrderManager syncOrderManager,
        IOrderManagementClientNotificationManager clientNotificationManager) 
    {
        CurrentDevice = device;
        OrderPipeline = orderPipeline;
        SyncOrderManager = syncOrderManager;
        ClientNotificationManager = clientNotificationManager;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing
    }

    /// <summary>
    /// Current order management messing instance
    /// </summary>
    public IOrderManagementClientNotificationManager ClientNotificationManager { get; set; }

    /// <summary>
    /// Current order pipeline
    /// </summary>
    public IOrderPipeline OrderPipeline { get; set; }

    /// <summary>
    /// Current sync order manager
    /// </summary>
    public ISyncOrderManager SyncOrderManager { get; set; }

    /// <summary>
    /// Current device server instance
    /// </summary>
    public IOrderManagementDevice CurrentDevice { get; set; }

    /// <summary>
    /// Is the runner stopped? 
    /// </summary>
    public bool IsRunnerStopped { get; set; }

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
    /// Get all orders in the waiting non-priority queue
    /// </summary>
    public IList<IOrder> OrdersInQueue { get; } = new List<IOrder>();

    /// <summary>
    /// Get all waiting but not cancelled orders in the priority queue
    /// </summary>
    public IList<IOrder> OrdersInQueueWithPriority { get; } = new List<IOrder>();

    /// <summary>
    /// Get all waiting but not cancelled orders currently in processing
    /// </summary>
    public IList<IOrder> OrdersInProcessing { get; } = new List<IOrder>();

    /// <summary>
    /// Get all orders currently in processing, in the queue and in priority line
    /// </summary>
    public IList<IOrder> AllOrders { get; } = new List<IOrder>();

    /// <summary>
    /// No order in processing
    /// </summary>
    public bool IsNoOrderInProcessing { get; set; }

    /// <summary>
    /// Is only one order in processing?
    /// </summary>
    public bool IsUniqueOrderInProcessing { get; set; }

    /// <summary>
    /// Is any order in queue to process
    /// </summary>
    public bool IsAnyOrderToProcess { get; set; }

    /// <summary>
    /// Is there an order running or in the queue breaking a device properties update
    /// </summary>
    public bool IsAnyOrderBreakingdevicePropertiesUpdateToProcess { get; set; }

    /// <summary>
    /// Get the current order processing
    /// </summary>
    /// <returns>Currently processed order</returns>
    public IOrder GetCurrentProcessingOrder()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the request processor
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    /// <returns>Request processor</returns>
    public IRequestProcessor GetRequestProcessorForOrder(long orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Number of orders in processing
    /// </summary>
    public int OrdersInProcessingCount { get; set; }

    /// <summary>
    /// Is a StSys init order in the queue or just executing
    /// </summary>
    public bool InitInTheQueue { get; set; }

    /// <summary>
    /// Is an init process processing. True while the orders of StSys init and hardware init are handled. Important to NOT start real order processing in <see cref="IOrderProcessor.Runner "/> before checkslot was handled
    /// </summary>
    public bool IsInitInProcessing { get; set; }

    /// <summary>
    /// Adds an order to the order processing queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the order queue</param>
    public void AddOrder(IOrder order)
    {
        AsyncHelper.FireAndForget(() =>
        {
            if (CurrentDevice is IStateMachineDevice { CurrentState: IOrderBasedActionStateMachineState obas })
            {
                obas.OrderFinishedSucessfully(order.Id);
            }
        });
    }

    /// <summary>
    /// Adds an order to the order with high priority processing queue for further processing
    /// this is meant for unload orders after hardware error
    /// </summary>
    /// <param name="order">Order to add to the order queue</param>
    public void AddOrderWithPriority(IOrder order)
    {
        AsyncHelper.FireAndForget(() =>
        {
            if (CurrentDevice is IStateMachineDevice { CurrentState: IOrderBasedActionStateMachineState obas })
            {
                obas.OrderFinishedSucessfully(order.Id);
            }
        });
    }

    /// <summary>
    /// Cancel all running orders and run a device order init if required
    /// </summary>
    /// <param name="errorCode">Provided error code from device</param>
    public void CancelRunningOrders(byte errorCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handle a received error message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public void HandleError(IInboundDataMessage receivedMessage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initiate a hardware init and run the order directly
    /// </summary>
    public IOrder InitiateHardwareInit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initiate a hardware init
    /// </summary>
    /// <param name="runTheOrder">Run the order directly: yes or no</param>
    /// <returns>device hardware init order</returns>
    public IOrder InitiateHardwareInit(bool runTheOrder)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all queued and currently running orders
    /// </summary>
    public void CancelAllOrders()
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
    /// Try to execute an order directly. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <returns>Order execution result</returns>
    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
    /// <returns>Order execution result</returns>
    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Try to execute an order directly. Method is intended for unit testing. Depending on order priority the order is loaded to the priority queue or not
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="doNotDisposeOrder">Do not dispose the order after running it (intended for unit testing)</param>
    /// <param name="directRunOrder">Run the roder directly without using queue</param>
    /// <returns>Order execution result</returns>
    public IOrderExecutionResultState TryToExecuteOrderSync(IOrder order, bool doNotDisposeOrder, bool directRunOrder)
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
    /// Cancel an order via its ID
    /// </summary>
    /// <param name="orderId">ID of the order to cancel</param>
    public void CancelOrder(long orderId)
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
    /// Executes the waiting orders. Method made public for easier unit testing.
    /// Do not use directly in production code!!!
    /// </summary>
    public void Runner()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public bool CheckReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    public void StartOrderProcessing()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    public void StopOrderProcessing()
    {
        throw new NotImplementedException();
    }
}