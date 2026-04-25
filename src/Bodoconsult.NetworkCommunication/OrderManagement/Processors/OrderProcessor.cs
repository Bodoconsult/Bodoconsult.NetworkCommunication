// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IOrderProcessor"/>. This implementation uses a polling mechanism to execute orders instead of producer-consumer-pattern. The reason is that multiple queues have to be handled
/// </summary>
public class OrderProcessor : BaseOrderProcessor
{
    private readonly IOnlyOrderManagementDevice _device;

    /// <summary>
    /// Call back method fired from request processor
    /// </summary>
    /// <param name="orderId"></param>
    protected override void OrderProcessingFinished(long orderId)
    {
        var requestProcessor = OrderPipeline.GetFromExecutionQueue(orderId);

        if (requestProcessor == null)
        {
            var erg = OrderExecutionResultState.Unsuccessful;

            StopExecutionOfSyncOrder(orderId, erg);

            AppLogger.LogDebug($"{LoggerId}order {orderId} has finished but request processor wasn't found");
            return;
        }

        // Deactivate order handling here
        requestProcessor.OrderProcessingFinishedDelegate = null;

        var order = requestProcessor.Order;
        order.Benchmark?.AddStep("Order processing finished");

        AppLogger.LogDebug($"{LoggerId}{order.LoggerId}has finished. {OrderPipeline.CurrentOrderState}");

        // Process successful orders
        if (order.ExecutionResult.Id == OrderExecutionResultState.Successful.Id)
        {
            _device.OrderFinishedSuccessful(order);
            order.Benchmark?.AddStep("Order finished Successful");
        }
        else // Process unsuccessful orders
        {
            _device.OrderFinishedUnsuccessful(order);
            order.Benchmark?.AddStep("Order finished Unsuccessful");
        }

        // Now clean the execution queue
        OrderPipeline.RemoveFromExecutionQueue(orderId);
        requestProcessor.Dispose();

        // Set the order running endtime
        order.EndTime = DateTimeService.Now;
        order.IsFinished = true;
        order.Benchmark?.AddStep("Disposing now");

        // Async order: dispose now

        if (!order.IsRunningSync)
        {
            order.Dispose();
        }
        else // Sync order: give control back to calling method
        {
            // Create the order execution result for a sync running order
            var erg = order.ExecutionResult;

            order.IsDisposable = true;
            StopExecutionOfSyncOrder(order.Id, erg);
        }

        Debug.Print($"TOP: {OrderPipeline.CurrentOrderState}");
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderProcessor(
        IOnlyOrderManagementDevice device,
        IAppDateService dateTimeService,
        IOrderPipeline orderPipeline,
        ISyncOrderManager syncOrderManager,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IAppBenchProxy appBenchProxy) : base(device, dateTimeService, orderPipeline, syncOrderManager, clientNotificationManager, appBenchProxy)
    {
        _device = device;
    }

    /// <summary>
    /// Initiate a hardware init
    /// </summary>
    /// <param name="runTheOrder">Run the order directly: yes or no</param>
    /// <returns>device hardware init order</returns>
    public override IOrder? InitiateHardwareInit(bool runTheOrder)
    {
        // Wait until all running orders are ready for disposing finally
        var orders = OrdersInProcessing;

        if (orders.Any())
        {
            using var cts = new CancellationTokenSource(5000);
            while (!cts.IsCancellationRequested)
            {
                if (orders.All(x => x.IsDisposable))
                {
                    break;
                }

                Thread.Sleep(5);
            }
        }

        AppLogger.LogInformation($"{LoggerId}execution queue cleared for device hardware init");

        // Is no hardware init required set externally for unit testing
        if (IsNoHardWareInitRequired)
        {
            AppLogger.LogDebug($"{LoggerId}runner restarted");
            IsRunnerStopped = false;
            return null;
        }

        //// Check if already a hardware init is running
        //foreach (var execOrder in OrdersInProcessing)
        //{
        //    if (execOrder.TypeName == nameof(deviceHardwareInitOrder))
        //    {
        //        return null;
        //    }
        //}

        // Create the device hardware init 
        var order = CurrentDevice.CreateHardwareInitOrder();

        IsInitInProcessing = true;
        IsRunnerStopped = true;

        CheckAndRunOrder(order);

        _device.DoNotifyHardwareInitRequested();
        AppLogger.LogInformation($"{LoggerId}{order.LoggerId}order execution started");

        return order;
    }

    /// <summary>
    /// Executes the waiting orders. Method made public for easier unit testing.
    /// Do not use directly in production code!!!
    /// </summary>
    public override void Runner()
    {
        if (!OrderPipeline.IsRunningOrdersEmpty)
        {
            OrderPipeline.RemoveCancelledOrdersFromExecutionQueue();
        }

        // Check conditions in order processor if running an order is allowed
        var isNoOrderWaiting = OrderPipeline is { IsWaitingOrdersEmpty: true, IsWaitingPriorityOrdersEmpty: true };
        if (IsRunnerStopped || isNoOrderWaiting)
        {
            if (isNoOrderWaiting && !IsInitInProcessing)
            {
                _device.CheckIfThereAreOrdersToBeCreated();
            }

            //LogInformation($"IsRunnerStopped {IsRunnerStopped}: IsNoOrderWaiting: {isNoOrderWaiting} IsInitInProcessing: {IsInitInProcessing}");
            return;
        }

        // Check if the device is ready for order running
        if (!_device.IsRunningOrdersAllowed)
        {
            //LogInformation($"IsRunningOrdersAllowed: {_device.IsRunningOrdersAllowed}");
            return;
        }

        // Get the next order to run from business logic
        var order = _device.GetNextOrderToRun();

        if (order == null)
        {
            //LogInformation("GetNextOrderToRun: null");
            return;
        }

        // Run the order now
        CheckAndRunOrder(order);

    }

    /// <summary>
    /// Check and then run the order
    /// </summary>
    /// <param name="order">Current order</param>
    public override void CheckAndRunOrder(IOrder order)
    {
        var runningOrders = OrderPipeline.RunningOrders.ToList();

        // Check if it is allowed to run the order now
        if (!_device.IsRunningTheOrderAllowed(order, runningOrders))
        {
            return;
        }

        // Dequeue the order now go get it out pipeline
        if (OrderPipeline.DequeueOrder(order))
        {
            return;
        }

        // last check to prevent from deadlock
        _device.Check4ConcurrentOrders(order);

        // Now finally execute the order
        RunOrder(order);
    }


    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public override bool CheckReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        if (receivedMessage == null)
        {
            return false;
        }

        //if (receivedMessage is not IInboundDataMessage rm)
        //{
        //    msg = $"received {receivedMessage.ToInfoString()} {OrderPipeline.CurrentOrderState}";
        //    LogInformation(msg);
        //    return false;
        //}

        Debug.Print($"TOP: message received: {receivedMessage.ToInfoString()}");

        //*********************
        // TOP 1 A X message with a error code of 0 makes no sense: throw this message away
        // This other X messages should be given to the order pipeline first and after that it should be handled befor given to async message handler
        //*********************
        if (CurrentDevice.DoBasicCheckForReceivedMessage(receivedMessage))
        {
            return true;
        }

        var msg = $"received {receivedMessage.ToInfoString()} {OrderPipeline.CurrentOrderState}";
        LogInformation(msg);

        //*********************
        // TOP 2 Deliver message to order pipeline with all running orders with higher priority to check
        //*********************
        var requestProcessors = OrderPipeline.RunningRequestProcessors;
        if (CheckOrders(requestProcessors.Where(x => x.Order is { IsCheckedWithPriority: true, WasSuccessful: false, IsFinished: false }), receivedMessage))
        {
            return true;
        }

        //*********************
        // TOP 3 Deliver message to order pipeline with all running orders with normal priority to check
        //*********************
        if (CheckOrders(requestProcessors.Where(x => x.Order is { IsCheckedWithPriority: false, WasSuccessful: false, IsFinished: false }), receivedMessage))
        {
            return true;
        }

        //*********************
        // TOP 4 Handle an error message now if not handled by any order before
        //*********************
        if (CurrentDevice.DoCheckForErrorMessage(receivedMessage))
        {
            return true;
        }

        //*********************
        // TOP 5 Last chance for message handling: async message handling
        //*********************
        var result = _device.DataMessagingConfig.AppLayerHandleAsyncMessageDelegate?.Invoke(receivedMessage);

        //IsRunnerStopped = false;

        //if (result == null)
        //{
        //    msg = $"{receivedMessage.ToShortInfoString()}: async processed unsuccessful. Message is disposed now";
        //    LogInformation(msg);
        //    return false;
        //}

        msg = $"{receivedMessage.ToShortInfoString()}: async processed {result.ExecutionResult}{(result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id ? "" : ". Message is disposed now")}";
        LogInformation(msg);

        return result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id;
    }

    /// <summary>
    /// Handle a received error
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public override void HandleError(IInboundMessage receivedMessage)
    {
        if (receivedMessage is not IInboundDataMessage msg)
        {
            return;
        }

        _device .NoStateMachineHandleErrorMessageDelegate?.Invoke(msg);
    }






}