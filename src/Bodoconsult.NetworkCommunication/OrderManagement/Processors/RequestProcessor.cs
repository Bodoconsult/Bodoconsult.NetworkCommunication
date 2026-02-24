// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IRequestProcessor"/>
/// </summary>
public class RequestProcessor : IRequestProcessor
{
    private readonly IRequestStepProcessorFactory _requestStepProcessorFactory;
    private readonly IOrderManagementDevice _device;
    private bool _isCancelled;
    private readonly Lock _cancelLockObject = new();
    private readonly Lock _stepProcessorObject = new();
    private readonly string _orderLoggerId;
    private object _transportObject;
    private bool _isDisposing;
    private bool _isExitActionFired;
    private readonly Lock _isExitActionFiredLock = new();
    private IRequestStepProcessor _currentRequestStepProcessor;
    private readonly IAppLoggerProxy _appLogger;

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    public bool IsCancelled
    {
        get
        {
            lock (_cancelLockObject)
            {
                return _isCancelled;
            }
        }
        private set
        {
            lock (_cancelLockObject)
            {
                _isCancelled = value;
            }
        }
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="order">Current order to process</param>
    /// <param name="requestStepProcessorFactory">Current factory to create <see cref="IRequestStepProcessor"/> instances.</param>
    /// <param name="device">Current device</param>
    public RequestProcessor(IOrder order, IRequestStepProcessorFactory requestStepProcessorFactory, IOrderManagementDevice device)
    {
        Order = order;
        Order.WasSuccessful = false;
        _requestStepProcessorFactory = requestStepProcessorFactory;
        _device = device ?? throw new ArgumentNullException(nameof(device));

        if (_device.DataMessagingConfig == null)
        {
            return;
        }
        _orderLoggerId = $"{_device.DataMessagingConfig.LoggerId}{Order.LoggerId}";
        _appLogger = _device.DataMessagingConfig.AppLogger;
    }

    /// <summary>
    /// Current order to process
    /// </summary>
    public IOrder Order { get; private set; }

    /// <summary>
    /// The currently processed request step of the order
    /// </summary>
    public IRequestStepProcessor CurrentRequestStepProcessor
    {
        get
        {
            lock (_stepProcessorObject)
            {
                return _currentRequestStepProcessor;
            }
        }
        private set
        {
            lock (_stepProcessorObject)
            {
                _currentRequestStepProcessor = value;
            }
        }
    }

    /// <summary>
    /// Execute the order request by request
    /// </summary>
    /// <returns>Execution result</returns>
    public IOrderExecutionResultState ExecuteOrder()
    {
        // Fetch the order here to avoid multithread issues
        var order = Order;

        //try
        //{
        if (order == null || order.IsCancelled || _isDisposing)
        {
            return ExitAction(order, OrderExecutionResultState.Unsuccessful);
        }

        // Run all request specs
        foreach (var requestSpec in order.RequestSpecs)
        {

            if (order.IsDisposable)
            {
                return ExitAction(order, OrderExecutionResultState.Unsuccessful);
            }

            if (IsCancelled || order.IsCancelled)
            {
                _appLogger?.LogInformation($"{_orderLoggerId}{requestSpec.Name} was cancelled");
                return ExitAction(order, OrderExecutionResultState.Unsuccessful);
            }

            Debug.Print($"RP: {requestSpec.GetType().Name}: start execution...");

            var result = ExecuteRequest(requestSpec);

            Debug.Print($"RP: {requestSpec.GetType().Name}: execution ended with {result}");

            if (result.Id == OrderExecutionResultState.Successful.Id && !IsCancelled && !order.IsCancelled)
            {
                continue;
            }

            if (IsCancelled || order.IsCancelled)
            {
                _appLogger?.LogInformation($"{_orderLoggerId} was cancelled");
                return ExitAction(order, OrderExecutionResultState.Unsuccessful);
            }

            _appLogger?.LogInformation($"{_orderLoggerId}exit {requestSpec.Name} with code {result}");
            return ExitAction(order, result);

        }

        _appLogger?.LogInformation($"{_orderLoggerId} finished successful");

        // If all requests were successful, the complete order was successful
        if (order.RequestSpecs.All(x => x.WasSuccessful))
        {
            order.WasSuccessful = true;
        }

        return ExitAction(order, OrderExecutionResultState.Successful);
        //}
        //catch (Exception e)
        //{
        //    _deviceServer.Smddevice.AppLogger.LogError($"{_deviceServer.Smddevice.LoggerId}{order?.LoggerId} executing failed", e);
        //    return ExitAction(order, OrderExecutionResultState.Unsuccessful);
        //}
    }

    /// <summary>
    /// Prepare the request processor for correct exit
    /// </summary>
    /// <param name="order">The current order</param>
    /// <param name="currentState">Current state of the order</param>
    /// <returns>Current state of the order</returns>
    private IOrderExecutionResultState ExitAction(IOrder order, IOrderExecutionResultState currentState)
    {
        if (order != null)
        {
            // Safety check
            if (!order.WasSuccessful && currentState.Id == OrderExecutionResultState.Successful.Id)
            {
                order.WasSuccessful = true;
            }

            lock (_isExitActionFiredLock)
            {
                if (_isExitActionFired)
                {
                    return order.ExecutionResult;
                }
                _isExitActionFired = true;
            }
        }

        CurrentRequestStepProcessor?.Cancel();

        // If the order has been finished already or is disposable: do not change order state again
        if (order == null ||
            order.IsFinished ||
            order.IsDisposable)
        {
            currentState = order?.ExecutionResult ?? currentState;
        }
        else
        {
            order.ExecutionResult = currentState;

            // Inform device order processor: order is done
            OrderProcessingFinishedDelegate?.Invoke(order.Id);
        }

        // Deactivate informing device order processor for safety reasons. OrderProcessingFinished should not be called more than one times
        CurrentRequestStepProcessor = null;
        Order = null;
        OrderProcessingFinishedDelegate = null;
        return currentState;
    }

    /// <summary>
    /// Execute a single step
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <returns>Execution result</returns>
    public IOrderExecutionResultState ExecuteRequest(IRequestSpec requestSpec)
    {
        // Fetch the order here to avoid multithread issues
        var order = Order;

        if (order == null || IsCancelled || order.IsCancelled)
        {
            return SetUnsuccessful(order);
        }

        requestSpec.DoNotifyDelegate = _device.DoNotify;
        requestSpec.SendDataMessageDelegate = _device.CommunicationAdapter.SendDataMessage;
        requestSpec.CancelRunningOperationDelegate = _device.CommunicationAdapter.CancelRunningOperation;
        requestSpec.AppLogger = _appLogger;
        requestSpec.OrderLoggerId = $"{_orderLoggerId}RSP: {requestSpec.Name} ";
        
        var processor = _requestStepProcessorFactory.CreateProcessor(requestSpec, _device);
        requestSpec.RequestStepProcessorSetResultDelegate = processor.SetResult;
        requestSpec.RequestStepProcessorIsCancelledDelegate = processor.CheckIsCancelled;
        processor.PrepareTheChain();

        processor.RequestSpec.SetTransportObject(_transportObject);

        CurrentRequestStepProcessor = processor;

        if (IsCancelled || order.IsCancelled)
        {
            processor.Cancel();
            return SetUnsuccessful(order);
        }

        var result = processor.ExecuteRequest();

        if (result.Id == OrderExecutionResultState.Successful.Id)
        {
            _transportObject = processor.RequestSpec.ResultTransportObject;
            processor.Dispose();
            Debug.Print("ExecuteRequest successful");
            return OrderExecutionResultState.Successful;
        }

        // ToDo: add logging

        Debug.Print("ExecuteRequest failed");

        // If the order has been finished already or is disposable: do not change order state again
        if (order.IsFinished || order.IsDisposable)
        {
            return order.ExecutionResult;
        }

        order.ExecutionResult = result;
        return result;
    }

    /// <summary>
    /// Set the new unsucessful state only if the order is still in successful state
    /// </summary>
    /// <param name="order">Current order</param>
    /// <returns>Order execution result state</returns>
    private static IOrderExecutionResultState SetUnsuccessful(IOrder order)
    {
        if (order.ExecutionResult.Id == OrderExecutionResultState.Successful.Id)
        {
            order.ExecutionResult = OrderExecutionResultState.Unsuccessful;
        }

        return order.ExecutionResult;
    }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public bool CheckReceivedMessage(IInboundDataMessage receivedMessage)
    {
        //try
        //{

        // Fetch the order here to avoid multithread issues
        var order = Order;

        _device.DataMessagingConfig.AppLogger.LogDebug($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} with order ID {order?.Id ?? 0}!");

        if (order == null || _isDisposing || IsCancelled)
        {
            //Order.ExecutionResult = OrderExecutionResultState.Unsuccessful;
            return false;
        }

        var isDisposable = order.IsDisposable;
        var isFinished = order.IsFinished;
        var isCancelled = order.IsCancelled;


        if (isCancelled)
        {
            //Order.ExecutionResult = OrderExecutionResultState.Unsuccessful;
            _device.DataMessagingConfig.AppLogger.LogDebug($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} failed: order is cancelled already");
            return false;
        }


        var stepProcessor = CurrentRequestStepProcessor;
        if (stepProcessor == null)
        {
            //Order.ExecutionResult = OrderExecutionResultState.Unsuccessful;
            _device.DataMessagingConfig.AppLogger.LogDebug($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} failed: no request step processor");
            return false;
        }

        var result = stepProcessor.CheckReceivedMessage(receivedMessage);


        // If the order has been finished already or is disposable: do not change order state again
        if (isDisposable || isFinished)
        {
            _device.DataMessagingConfig.AppLogger.LogDebug($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} failed: order is disposable or finished already");
            return result;
        }


        if (order.RequestSpecs.All(x => x is { WasSuccessful: true }))
        {
            order.WasSuccessful = true;
        }

        if (!order.WasSuccessful)
        {
            _device.DataMessagingConfig.AppLogger.LogDebug($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} result: {result}");
            return result;
        }

        //order.ExecutionResult = OrderExecutionResultState.Successful;
        return result;

        //}
        //catch (Exception e)
        //{
        //    _deviceServer.Smddevice.AppLogger.LogError($"{_orderLoggerId}receiving message {receivedMessage.ToShortInfoString()} failed", e);
        //    return false;
        //}
    }

    /// <summary>
    /// Cancel a not running processor
    /// </summary>
    public void Cancel()
    {
        Cancel(false, false);
    }

    /// <summary>
    /// Cancel the processor
    /// </summary>
    /// <param name="isRunning">Is the order running at the moment</param>
    /// <param name="isHardwareError">Is the reason for cancelling a hardware error</param>
    public void Cancel(bool isRunning, bool isHardwareError)
    {
        if (IsCancelled)
        {
            return;
        }

        // Cancel running order in the request step processor
        var rsp = CurrentRequestStepProcessor;
        if (isRunning && rsp != null)
        {
            rsp.Cancel();
            Wait.Until(() => rsp.IsCancelled, 100);
        }

        // Keep order instance for the runtime of this method
        var order = Order;

        if (order == null)
        {
            return;
        }

        IsCancelled = true;
        order.IsCancelled = true;
        order.ExecutionResult = OrderExecutionResultState.Unsuccessful;
        ExitAction(order, OrderExecutionResultState.Unsuccessful);

        if (!isRunning)
        {
            return;
        }

        // avoid cancellation of operation if not required or not possible
        if (isHardwareError || !order.SendCancelTodeviceIfCancelledOrUnsuccessful)
        {
            return;
        }

        _device.CancelRunningOperation();
    }

    /// <summary>
    /// A delegate to implement a call back to say the <see cref="IOrderProcessor"/> that order is processed
    /// </summary>
    public OrderProcessingFinishedDelegate OrderProcessingFinishedDelegate { get; set; }

    /// <summary>
    /// The task the <see cref="IRequestProcessor.ExecuteOrder"/> command is running in
    /// </summary>
    public Task CurrentTask { get; set; }

    /// <summary>
    /// Used to cancel <see cref="IRequestProcessor.CurrentTask"/> if required
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; } = new(5000);

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _isDisposing = true;
        Thread.Sleep(20);

        CurrentRequestStepProcessor?.Dispose();
        CurrentRequestStepProcessor = null;
        CurrentTask = null;
        _transportObject = null;
        // Reset values in the steps
        foreach (var requestSpec in Order.RequestSpecs)
        {
            foreach (var step in requestSpec.RequestAnswerSteps)
            {
                step.Dispose();
            }

            requestSpec.Dispose();
        }
        Order = null;
        OrderProcessingFinishedDelegate = null;
    }
}