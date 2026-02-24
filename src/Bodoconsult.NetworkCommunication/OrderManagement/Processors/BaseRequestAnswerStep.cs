// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a base class for request steps
/// </summary>
public class BaseRequestAnswerStep : IRequestAnswerStep
{
    private bool _wasSuccessful;
    private readonly Lock _wasSuccessfulLock = new();

    private TaskCompletionSource<IOrderExecutionResultState> _taskCompletionSource;
    private CancellationTokenSource _ctsMain;

    private readonly IOrder _order;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public BaseRequestAnswerStep(IRequestSpec requestSpec)
    {
        RequestSpec = requestSpec ?? throw new ArgumentNullException(nameof(requestSpec));

        if (RequestSpec.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(RequestSpec.ParameterSet));
        }

        _order = RequestSpec.ParameterSet.CurrentOrder ?? throw new ArgumentNullException(nameof(RequestSpec.ParameterSet.CurrentOrder));
    }

    /// <summary>
    /// Current request spec
    /// </summary>
    public IRequestSpec RequestSpec { get; }

    /// <summary>
    /// Allowed request answers: one of the items has to be received as answer from tower
    /// </summary>
    public List<IRequestAnswer> AllowedRequestAnswers { get; } = new();

    /// <summary>
    /// Cancel the step
    /// </summary>
    public virtual void Cancel()
    {
        Debug.Print("RAS: cancel");
        SetResult(OrderExecutionResultState.Unsuccessful);
    }

    /// <summary>
    /// Total timeout for the answer(s) of a request in milliseconds
    /// </summary>
    public int Timeout { get; set; } = DeviceCommunicationBasics.DefaultTimeout;

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    public bool WasSuccessful
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

                _order.WasSuccessful = false;
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
            if (_order.RequestSpecs.All(x => x is { WasSuccessful: true }))
            {
                _order.WasSuccessful = true;
            }
        }
    }

    /// <summary>
    /// The next step in the chain
    /// </summary>
    public IRequestAnswerStep NextChainElement { get; set; }

    /// <summary>
    /// Delegate for action after cancelling the request step
    /// </summary>
    public ActionRequestStepDelegate CancelActionDelegate { get; set; }

    /// <summary>
    /// Delegate for action before the step is executed
    /// </summary>
    public ActionRequestStepDelegate BeforeExecuteActionDelegate { get; set; }

    /// <summary>
    /// Delegate for doing something if a RequestAnswerStep failed
    /// </summary>
    public HandleRequestAnswerStepFailedDelegate HandleRequestAnswerStepFailedDelegate { get; set; }

    /// <summary>
    /// The state to notify the app before the step is running
    /// </summary>
    public IDeviceState StateToNotifyBeforeRunning { get; set; } = DefaultDeviceStates.DeviceStateOffline;

    /// <summary>
    /// Defines a list of async received command who break or unbreak the current request step
    /// </summary>
    public IList<char> BreakingAsyncCommands { get; } = new List<char>();

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

        var success = CheckReceivedMessage(RequestSpec.CurrentSentMessage, receivedMessage, errors);
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
            if (!answer.CheckReceivedMessage(sentMessage, receivedMessage, errors))
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
    /// Handles the answer of the request
    /// </summary>
    /// <returns>Message handling result</returns>
    public virtual MessageHandlingResult HandleResult()
    {
        var requestSpec = RequestSpec;

        if (requestSpec == null)
        {
            return new MessageHandlingResult
            {
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

        var answer = requestSpec.IsInternalRequest ? AllowedRequestAnswers.FirstOrDefault() : AllowedRequestAnswers.FirstOrDefault(x => x.WasReceived);

        if (answer == null)
        {
            return new MessageHandlingResult
            {
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "received msg doesn't fit"
            };
        }

        // If no answer on success delegate delivered leave here
        if (answer.HandleRequestAnswerOnSuccessDelegate == null)
        {
            return new MessageHandlingResult
            {
                ExecutionResult = OrderExecutionResultState.Successful
            };
        }

        var run = 0;
        while (true)
        {

            // Handle the answer on success delegate
            if (!requestSpec.IsInternalRequest)
            {
                if (requestSpec.RequestStepProcessorIsCancelledDelegate() || _order.IsFinished || _order.IsCancelled)
                {
                    return new MessageHandlingResult
                    {
                        ExecutionResult = _order.ExecutionResult
                    };
                }
            }

            MessageHandlingResult result;
            try
            {
                Debug.Print($"{answer.HandleRequestAnswerOnSuccessDelegate.Method.Name}");
                Debug.Print("Start delegate...");
                result = answer.HandleRequestAnswerOnSuccessDelegate.Invoke(answer.ReceivedMessage, requestSpec.TransportObject, requestSpec.ParameterSet);
                Debug.Print("End delegate...");
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
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "run > 1"
            };
        }
    }

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    public virtual void Reset()
    {
        WasSuccessful = false;
        RequestSpec.RequestStepProcessorSetResultDelegate.Invoke(OrderExecutionResultState.Unsuccessful);

        foreach (var answer in AllowedRequestAnswers)
        {
            answer.Reset();
        }
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

        RequestSpec.AppLogger.LogDebug($"Left waiting. Order: {_order.Id} Step: {AllowedRequestAnswers.Count} StepSuccessful {WasSuccessful} Result: {taskResult}");

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
            RequestSpec.AppLogger.LogInformation($"{_order.LoggerId}RequestAnswerStep {this}: handle business logic for step was unsuccessful: {result.ErrorDescription}");
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


    private void SetResult(IOrderExecutionResultState result)
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

    private void SendRequestForNextReceivedMessage()
    {
        if (RequestSpec == null)
        {
            return;
        }

        // For testing purposes only: call the next answer data
        if (RequestSpec.IsInternalRequest || RequestSpec.RequestAnswerStepIsStartedDelegate == null)
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

        Thread.Sleep(RequestStepProcessor.WaitInterval);
    }



    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
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
}