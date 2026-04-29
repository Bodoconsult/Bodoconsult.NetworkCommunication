// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a device bound request step
/// </summary>
public class DeviceRequestAnswerStep : BaseRequestAnswerStep, IDeviceRequestAnswerStep
{
    private TaskCompletionSource<IOrderExecutionResultState>? _taskCompletionSource;
    private bool _wasSuccessful;
    private readonly Lock _wasSuccessfulLock = new();
    private CancellationTokenSource? _ctsMain;
    private readonly string _orderLoggerId;

    /// <summary>
    /// Current order
    /// </summary>
    protected readonly IOrder Order;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public DeviceRequestAnswerStep(IDeviceRequestSpec requestSpec) : base(requestSpec)
    {
        if (RequestSpec.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(RequestSpec.ParameterSet));
        }

        DeviceRequestSpec = requestSpec;
        Order = RequestSpec.ParameterSet.CurrentOrder ?? throw new ArgumentNullException(nameof(RequestSpec.ParameterSet.CurrentOrder));
        _orderLoggerId = Order.LoggerId;
    }

    /// <summary>
    /// Device bound request spec
    /// </summary>
    public IDeviceRequestSpec DeviceRequestSpec { get; }

    /// <summary>
    /// Handles the answer of the request
    /// </summary>
    /// <returns>Message handling result</returns>
    public override MessageHandlingResult HandleResult()
    {
        var requestSpec = RequestSpec;

        //if (requestSpec == null)
        //{
        //    return new MessageHandlingResult
        //    {
        //        Error = 1,
        //        ExecutionResult = OrderExecutionResultState.Unsuccessful,
        //        ErrorDescription = "No RequestSpec instance"
        //    };
        //}

        //var rsp = requestSpec.CurrentRequestStepProcessor;
        //if (rsp == null)
        //{
        //    return new MessageHandlingResult
        //    {
        //        ExecutionResult = OrderExecutionResultState.Unsuccessful,
        //        ErrorDescription = "No CurrentRequestStepProcessor instance"
        //    };
        //}

        var answer = AllowedRequestAnswers.FirstOrDefault(x => x.WasReceived);

        if (answer == null)
        {
            return new MessageHandlingResult
            {
                Error = 2,
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "received msg doesn't fit"
            };
        }

        //// If no answer on success delegate delivered leave here
        //if (answer.HandleRequestAnswerOnSuccessDelegate == null)
        //{
        //    return new MessageHandlingResult
        //    {
        //        ExecutionResult = OrderExecutionResultState.Successful
        //    };
        //}

        var run = 0;
        while (true)
        {
            //if (Order == null)
            //{
            //    return new MessageHandlingResult
            //    {
            //        ExecutionResult = OrderExecutionResultState.Unsuccessful
            //    };
            //}

            if (requestSpec.RequestStepProcessorIsCancelledDelegate?.Invoke() ?? Order.IsFinished || Order.IsCancelled)
            {
                return new MessageHandlingResult
                {
                    ExecutionResult = Order.ExecutionResult
                };
            }

            MessageHandlingResult result;
            try
            {
                if (answer.ReceivedMessage == null)
                {
                    return new MessageHandlingResult
                    {
                        Error = 4,
                        ExecutionResult = OrderExecutionResultState.Unsuccessful,
                        ErrorDescription = "Received message is null"
                    };
                }

                if (answer.HandleRequestAnswerOnSuccessDelegate == null)
                {
                    //return new MessageHandlingResult
                    //{
                    //    Error = 5,
                    //    ExecutionResult = OrderExecutionResultState.Unsuccessful,
                    //    ErrorDescription = "HandleRequestAnswerOnSuccessDelegate is null"
                    //};

                    return new MessageHandlingResult
                    {
                        Error = 0,
                        ExecutionResult = OrderExecutionResultState.Successful
                    };
                }
                Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name}Start delegate {answer.HandleRequestAnswerOnSuccessDelegate.Method.Name}...");
                result = answer.HandleRequestAnswerOnSuccessDelegate.Invoke(answer.ReceivedMessage, requestSpec.TransportObject, requestSpec.ParameterSet);
                Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name}End delegate...");
            }
            catch (Exception e)
            {
                return new MessageHandlingResult
                {
                    ExecutionResult = OrderExecutionResultState.Unsuccessful,
                    ErrorDescription = e.ToString()
                };
            }

            if (result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id)
            {
                // Return the result directly to transport the TransportObject to the next step
                return result;
            }

            run++;
            if (run <= 1)
            {
                continue;
            }

            return new MessageHandlingResult
            {
                Error = 3,
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "run > 1"
            };
        }
    }

    /// <summary>
    /// Check the received message is fitting to the current request
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the received message is fitting to the current request else false</returns>
    public IEnumerable<string> CheckReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        ArgumentNullException.ThrowIfNull(DeviceRequestSpec.CurrentSentMessage);

        var errors = new List<string>();

        //Trace.TraceInformation("{DeviceRequestSpec.Name} CheckReceivedMessage");

        //#if DEBUG
        //            var i = RequestSpec.RequestAnswerSteps.IndexOf(this);

        //            Trace.TraceInformation($"{DeviceRequestSpec.Name}CheckReceivedMessage step: {i}");
        //            foreach (var s in AllowedRequestAnswers)
        //            {
        //                Trace.TraceInformation($"{DeviceRequestSpec.Name}CheckReceivedMessage step: {i}: {s.Command}");
        //            }
        //#endif


        var success = CheckReceivedMessage(DeviceRequestSpec.CurrentSentMessage, receivedMessage, errors);
        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.OrderLoggerId}RSP: check message: command {receivedMessage?.ToInfoString()}: {success} ");

        // Check if received message is a breaking async message
        if (receivedMessage != null && !success)
        {
            //lock (_isBreakLock)
            //{
            //    _isBreak = true;
            //}

            errors.Add($"Do not process the message {receivedMessage.MessageId} here. Let it proceed to async message handling");

            // Only for tests!!!!
            SendRequestForNextReceivedMessage();

            return errors;

            //Trace.TraceInformation($"Break: {_isBreak}");
            // Do not process the async message here. Let it proceed to AsyncMessageHandler
        }

        // if no success leave here
        if (!success)
        {
            return errors;
        }

        WasSuccessful = true;
        SetResult(OrderExecutionResultState.Successful);
        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name} successful");

        //Trace.TraceInformation($"RSP: waiting for action result finished: {_isStepDone} ");
        return errors;
    }

    private void SendRequestForNextReceivedMessage()
    {
        //if (RequestSpec == null)
        //{
        //    return;
        //}

        // For testing purposes only: call the next answer data
        if (RequestSpec.RequestAnswerStepIsStartedDelegate == null)
        {
            return;
        }

        AsyncHelper.Delay(200);

        if (_taskCompletionSource == null || _taskCompletionSource.Task.IsCanceled)
        {
            return;
        }

        AsyncHelper.FireAndForget(() =>
        {
            RequestSpec.RequestAnswerStepIsStartedDelegate.Invoke();
        });

        Thread.Sleep(DeviceRequestStepProcessor.WaitInterval);
    }

    /// <summary>
    /// Check if a received message is the expected answer to the request step.
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the tower</param>
    /// <param name="receivedMessage">A received message from the tower</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    public virtual bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors)
    {
        if (AllowedRequestAnswers.Any(x => x.WasReceived))
        {
            return false;
        }

        foreach (var answer in AllowedRequestAnswers)
        {
            if (!answer.CheckReceivedMessageDelegate?.Invoke(answer, sentMessage, receivedMessage, errors) ?? false)
            {
                continue;
            }
            Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name}: CheckReceivedMessage: answer {answer.GetType().Name} successful");
            AcceptedMessage = receivedMessage;
            return true;
        }

        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name}: CheckReceivedMessage: step: not successful");
        return false;
    }

    /// <summary>
    /// The accepted message leading <see cref="IRequestAnswerStep.WasSuccessful"/> being true
    /// </summary>
    public IInboundDataMessage? AcceptedMessage { get; protected set; }

    /// <summary>
    /// Next chain element
    /// </summary>
    public IDeviceRequestAnswerStep? NextChainElement { get; set; }

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    public override bool WasSuccessful
    {
        get
        {
            lock (_wasSuccessfulLock)
            {
                return _wasSuccessful;
            }
        }
        set
        {
            lock (_wasSuccessfulLock)
            {
                _wasSuccessful = value;
            }

            //if (DeviceRequestSpec == null)
            //{
            //    return;
            //}

            if (!value)
            {
                DeviceRequestSpec.WasSuccessful = false;
                DeviceRequestSpec.RequestStepProcessorSetResultDelegate?.Invoke(OrderExecutionResultState.Unsuccessful);

                Order.WasSuccessful = false;
                return;
            }

            RequestSpec.RequestStepProcessorSetResultDelegate?.Invoke(OrderExecutionResultState.Successful);

            // Check if all steps are done. If yes the step was done successfully
            if (!DeviceRequestSpec.RequestAnswerSteps.All(x => x.WasSuccessful))
            {
                return;
            }

            RequestSpec.WasSuccessful = true;

            // Check now if the order was successful
            if (Order.RequestSpecs.All(x => x is { WasSuccessful: true }))
            {
                Order.WasSuccessful = true;
            }
        }
    }

    /// <summary>
    /// Cancel the step
    /// </summary>
    public override void Cancel()
    {
        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name} cancel");
        SetResult(OrderExecutionResultState.Unsuccessful);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public override void Dispose()
    {
        //var rsp = RequestSpec.CurrentRequestStepProcessor;

        //if (rsp != null)
        //{
        //    rsp.AppLogger?.LogDebug($"{rsp.OrderLoggerId}Request step processor disposed at step {this}");
        //}

        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name} dispose");
        //Trace.TraceInformation($"{DeviceRequestSpec.Name} dispose {Environment.StackTrace}");

        NextChainElement = null;

        foreach (var allowedRequestAnswer in AllowedRequestAnswers)
        {
            allowedRequestAnswer.Dispose();
        }

        AllowedRequestAnswers.Clear();

        if (_taskCompletionSource is { Task: { IsCompleted: false } })
        {
            _taskCompletionSource.TrySetResult(OrderExecutionResultState.Unsuccessful);
        }

        _taskCompletionSource = null;
        _ctsMain?.Dispose();
        _ctsMain = null;
    }

    /// <summary>
    /// Process the current request answer step in a chain
    /// </summary>
    public void ProcessChainElement()
    {
        WasSuccessful = false;

        //var rsp = RequestSpec.CurrentRequestStepProcessor;

        //if (rsp == null)
        //{
        //    return;
        //}

        //try
        //{

        Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name} start with timeout of {Timeout} ms");

        // Now wait for incoming messages (doing it in a non-blocking mannor)
        var taskResult = AsyncHelper.RunSync(CreateWaitingTask);

        _taskCompletionSource = null;
        _ctsMain?.Dispose();
        _ctsMain = null;

        var msg = $"{Order.LoggerId}{DeviceRequestSpec.Name}: Left waiting. Order: {Order.Id} Step: {AllowedRequestAnswers.Count} StepSuccessful {WasSuccessful} Result: {taskResult}";
        Trace.TraceInformation(msg);
        RequestSpec.AppLogger?.LogDebug(msg);

        if (taskResult.Id != OrderExecutionResultState.Successful.Id)
        {
            WasSuccessful = false;
            RequestSpec.RequestStepProcessorSetResultDelegate?.Invoke(taskResult);
            return;
        }

        // Process the business logic for the step
        var result = HandleResult();

        if (result.ExecutionResult.Id != OrderExecutionResultState.Successful.Id)
        {
            RequestSpec.AppLogger?.LogInformation($"{Order.LoggerId}{DeviceRequestSpec.Name}: handle business logic for step was unsuccessful: {result.ErrorDescription}");
            WasSuccessful = false;

            RequestSpec.RequestStepProcessorSetResultDelegate?.Invoke(result.ExecutionResult);

            return;
        }

        // Deliver an existing transport object to the request spec to deliver it to the next step
        if (result.TransportObject != null)
        {
            RequestSpec.ResultTransportObject = result.TransportObject;
        }
    }

    /// <summary>
    /// Set the order execution result
    /// </summary>
    /// <param name="result">Order execution result</param>
    protected void SetResult(IOrderExecutionResultState result)
    {
        if (_taskCompletionSource == null)
        {
            return;
        }

        if (_taskCompletionSource.Task is { IsCompleted: false, IsCanceled: false })
        {
            _taskCompletionSource.SetResult(result);
        }
    }

    private Task<IOrderExecutionResultState>? CreateWaitingTask()
    {
        _ctsMain = new CancellationTokenSource(Timeout);
        _ctsMain.Token.Register(() =>
        {
            Trace.TraceInformation($"{_orderLoggerId}{DeviceRequestSpec.Name} timeout ({Timeout}ms)");
            SetResult(OrderExecutionResultState.Timeout);
        });

        _taskCompletionSource = new TaskCompletionSource<IOrderExecutionResultState>(TaskCreationOptions.RunContinuationsAsynchronously);

        SendRequestForNextReceivedMessage();

        return _taskCompletionSource?.Task ;
    }
}