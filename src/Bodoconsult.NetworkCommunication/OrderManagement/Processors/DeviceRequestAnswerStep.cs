// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a device bound request step
/// </summary>
public class DeviceRequestAnswerStep : BaseRequestAnswerStep, IDeviceRequestAnswerStep
{
    private TaskCompletionSource<IOrderExecutionResultState> _taskCompletionSource;
    private bool _wasSuccessful;
    protected readonly IOrder Order;
    private readonly Lock _wasSuccessfulLock = new();
    private CancellationTokenSource _ctsMain;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public DeviceRequestAnswerStep(IDeviceRequestSpec requestSpec) : base(requestSpec)
    {
        DeviceRequestSpec = requestSpec;
        Order = RequestSpec.ParameterSet.CurrentOrder ?? throw new ArgumentNullException(nameof(RequestSpec.ParameterSet.CurrentOrder));
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

        if (requestSpec == null)
        {
            return new MessageHandlingResult
            {
                Error = 1,
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "No RequestSpec instance"
            };
        }

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
            // Handle the answer on success delegate
            if (requestSpec.RequestStepProcessorIsCancelledDelegate() || Order.IsFinished || Order.IsCancelled)
            {
                return new MessageHandlingResult
                {
                    ExecutionResult = Order.ExecutionResult
                };
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
    public IEnumerable<string> CheckReceivedMessage(IInboundDataMessage receivedMessage)
    {
        var errors = new List<string>();

        Debug.Print("RAS: CheckReceivedMessage");

        //#if DEBUG
        //            var i = RequestSpec.RequestAnswerSteps.IndexOf(this);

        //            Debug.Print($"RAS.CheckReceivedMessage step: {i}");
        //            foreach (var s in AllowedRequestAnswers)
        //            {
        //                Debug.Print($"RAS.CheckReceivedMessage step: {i}: {s.Command}");
        //            }
        //#endif

        var success = CheckReceivedMessage(DeviceRequestSpec.CurrentSentMessage, receivedMessage, errors);
        //Debug.Print($"RSP: check message: command {receivedMessage?.ToInfoString()}: {success} ");

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

            //Debug.Print($"Break: {_isBreak}");
            // Do not process the async message here. Let it proceed to AsyncMessageHandler
        }

        // if no success leave here
        if (!success)
        {
            return errors;
        }

        WasSuccessful = true;
        SetResult(OrderExecutionResultState.Successful);
        Debug.Print("RAS: successful");

        //Debug.Print($"RSP: waiting for action result finished: {_isStepDone} ");
        return errors;
    }

    private void SendRequestForNextReceivedMessage()
    {
        if (RequestSpec == null)
        {
            return;
        }

        // For testing purposes only: call the next answer data
        if (RequestSpec.RequestAnswerStepIsStartedDelegate == null)
        {
            return;
        }

        AsyncHelper.Delay(200);

        if (_taskCompletionSource.Task.IsCanceled)
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
    public virtual bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors)
    {
        if (AllowedRequestAnswers.Any(x => x.WasReceived))
        {
            return false;
        }

        foreach (var answer in AllowedRequestAnswers)
        {
            if (!answer.CheckReceivedMessageDelegate.Invoke(answer, sentMessage, receivedMessage, errors))
            {
                continue;
            }
            Debug.Print($"RAS.CheckReceivedMessage answer: {answer.GetType().Name}");
            AcceptedMessage = receivedMessage;
            return true;
        }

        Debug.Print($"RAS.CheckReceivedMessage step: not successful");
        return false;
    }

    /// <summary>
    /// The accepted message leading <see cref="IRequestAnswerStep.WasSuccessful"/> being true
    /// </summary>
    public IInboundDataMessage AcceptedMessage { get; protected set; }

    /// <summary>
    /// Next chain element
    /// </summary>
    public IDeviceRequestAnswerStep NextChainElement { get; set; }

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

            if (RequestSpec == null)
            {
                return;
            }

            if (!value)
            {
                RequestSpec.WasSuccessful = false;
                RequestSpec.RequestStepProcessorSetResultDelegate.Invoke(OrderExecutionResultState.Unsuccessful);

                Order.WasSuccessful = false;
                return;
            }

            RequestSpec.RequestStepProcessorSetResultDelegate.Invoke(OrderExecutionResultState.Successful);

            // Check if all steps are done. If yes the step was done successfully
            if (!RequestSpec.RequestAnswerSteps.All(x => x.WasSuccessful))
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
        Debug.Print("RAS: cancel");
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

        Debug.Print($"RAS: dispose");
        //Debug.Print($"RAS: dispose {Environment.StackTrace}");

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
    public virtual void ProcessChainElement()
    {
        WasSuccessful = false;

        //var rsp = RequestSpec.CurrentRequestStepProcessor;

        //if (rsp == null)
        //{
        //    return;
        //}

        //try
        //{

        Debug.Print($"RAS: start with timeout of {Timeout} ms");

        // Now wait for incoming messages (doing it in a non-blocking mannor)
        var taskResult = AsyncHelper.RunSync(CreateWaitingTask);

        _taskCompletionSource = null;
        _ctsMain?.Dispose();
        _ctsMain = null;

        Debug.Print($"RAS: left waiting with result: {taskResult}");

        RequestSpec.AppLogger.LogDebug($"Left waiting. Order: {Order.Id} Step: {AllowedRequestAnswers.Count} StepSuccessful {WasSuccessful} Result: {taskResult}");

        if (taskResult.Id != OrderExecutionResultState.Successful.Id)
        {
            WasSuccessful = false;
            RequestSpec.RequestStepProcessorSetResultDelegate.Invoke(taskResult);
            return;
        }

        // Process the business logic for the step
        var result = HandleResult();

        if (result.ExecutionResult.Id != OrderExecutionResultState.Successful.Id)
        {
            RequestSpec.AppLogger.LogInformation($"{Order.LoggerId}RequestAnswerStep {this}: handle business logic for step was unsuccessful: {result.ErrorDescription}");
            WasSuccessful = false;

            RequestSpec.RequestStepProcessorSetResultDelegate.Invoke(result.ExecutionResult);

            return;
        }

        // Deliver an existing transport object to the request spec to deliver it to the next step
        if (result.TransportObject != null)
        {
            RequestSpec.ResultTransportObject = result.TransportObject;
        }
    }

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

    private Task<IOrderExecutionResultState> CreateWaitingTask()
    {
        _ctsMain = new CancellationTokenSource(Timeout);
        _ctsMain.Token.Register(() =>
        {
            Debug.Print($"RAS: timeout ({Timeout}ms)");
            SetResult(OrderExecutionResultState.Timeout);
        });

        _taskCompletionSource = new TaskCompletionSource<IOrderExecutionResultState>(TaskCreationOptions.RunContinuationsAsynchronously);

        SendRequestForNextReceivedMessage();

        return _taskCompletionSource?.Task;
    }
}