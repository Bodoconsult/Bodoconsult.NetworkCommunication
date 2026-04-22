// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IRequestStepProcessor"/> for device request steps
/// </summary>
public class DeviceRequestStepProcessor : IDeviceRequestStepProcessor
{
    private bool _isCancelled;
    private readonly Lock _isCancelledLockObject = new();
    private CancellationTokenSource? _tcs;
    private IDeviceRequestAnswerStep? _currentChainElement;
    private readonly Lock _currentChainElementObject = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceRequestStepProcessor(IDeviceRequestSpec requestSpec)
    {
        RequestSpec = requestSpec;
        DeviceRequestSpec = requestSpec;

        ArgumentNullException.ThrowIfNull(RequestSpec.AppLogger);
    }

    /// <summary>
    /// Wait interval in ms
    /// </summary>
    public static int WaitInterval { get; set; } = 20;

    /// <summary>
    /// Current device request spec
    /// </summary>
    public IDeviceRequestSpec DeviceRequestSpec { get; }

    /// <summary>
    /// The current processed chain element
    /// </summary>
    public IDeviceRequestAnswerStep? CurrentChainElement
    {
        get
        {
            lock (_currentChainElementObject)
            {
                return _currentChainElement;
            }
        }
        set
        {
            lock (_currentChainElementObject)
            {
                _currentChainElement = value;
            }
        }
    }

    /// <summary>
    /// Check if cancelled
    /// </summary>
    /// <returns>True if cancelled else false</returns>
    public bool CheckIsCancelled()
    {
        return _isCancelled;
    }

    /// <summary>
    /// Set the result state
    /// </summary>
    /// <param name="state">State to set as result</param>
    public void SetResult(IOrderExecutionResultState state)
    {
        Result = state;
    }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    public int NumberOfMessagesToBeSent { get; private set; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    public int CurrentNumberOfMessagesSent { get; private set; }

    /// <summary>
    /// Current request spec to use for the processor
    /// </summary>
    public IRequestSpec RequestSpec { get; set; }

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    public bool IsCancelled
    {
        get
        {
            lock (_isCancelledLockObject)
            {
                return _isCancelled;
            }
        }
        private set
        {
            lock (_isCancelledLockObject)
            {
                _isCancelled = value;
            }
        }
    }

    /// <summary>
    /// Current execution result
    /// </summary>
    public IOrderExecutionResultState Result { get; set; } = OrderExecutionResultState.Unsuccessful;

    /// <summary>
    /// Prepare the chain by creating the required elements
    /// </summary>
    public void PrepareTheChain()
    {
        for (var index = 0; index < DeviceRequestSpec.RequestAnswerSteps.Count; index++)
        {
            var step = (IDeviceRequestAnswerStep)DeviceRequestSpec.RequestAnswerSteps[index];

            if (index <= 0)
            {
                continue;
            }

            var elementBefore = (IDeviceRequestAnswerStep)DeviceRequestSpec.RequestAnswerSteps[index - 1];
            elementBefore.NextChainElement = step;
        }
    }

    /// <summary>
    /// Execute the request
    /// </summary>
    /// <returns>Execution result</returns>
    public IOrderExecutionResultState ExecuteRequest()
    {
        try
        {
            // Fetch the request spec here to avoid multithread issues

            var requestSpec = DeviceRequestSpec;


            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            var sendCancel = requestSpec.ParameterSet?.SendCancelToDeviceIfUnsuccessful ?? false;

            var repeatCount = 0;

            // Only request sending to device proceed here
            requestSpec.CreateMessagesToSend();


            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            var send = requestSpec.SentMessage.ToList();
            NumberOfMessagesToBeSent = send.Count;

            // Process all messages for the request in the same way
            foreach (var message in send)
            {
                // Repeat loop if necessary and if the execution result is NOT successful
                IOrderExecutionResultState result;
                while (true)
                {
                    if (IsCancelled)
                    {
                        return OrderExecutionResultState.Unsuccessful;
                    }

                    repeatCount++;
                    result = ExecuteRequest(message, requestSpec);

                    RequestSpec.AppLogger?.LogDebug($"{RequestSpec.OrderLoggerId}ExecuteRequest: {result} at {repeatCount} try");

                    if (result.Id == OrderExecutionResultState.Successful.Id ||
                        repeatCount >= requestSpec.NumberOfRepeatsInCaseOfNoSuccess)
                    {
                        break;
                    }

                    // Reset values in the steps
                    foreach (var step in requestSpec.RequestAnswerSteps)
                    {
                        step.Reset();
                    }

                    if (IsCancelled)
                    {
                        return OrderExecutionResultState.Unsuccessful;
                    }
                }

                if (result.Id == OrderExecutionResultState.Successful.Id)
                {
                    // ToDo: replace this Sleep(50) by a proper synchronization !!
                    // This sleep has been added because of a synchronization issue when more than one messages are added in a RequestSpec.
                    // In that case all messages inside RequestSpec are sent one after the other without waiting for sending their associated ACK message to the device. 
                    // => it might be not a big issue in a real life because apparently a device is not waiting any more for ACK messages ... but the device simulator did ... 
                    Thread.Sleep(WaitInterval);
                    CurrentNumberOfMessagesSent++;
                    continue;
                }

                if (sendCancel)
                {
                    RequestSpec.CancelRunningOperationDelegate?.Invoke();
                }

                return result;
            }

            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            // Check if all request steps are done. If yes the request spec was done successfully
            if (!requestSpec.RequestAnswerSteps.All(x => x.WasSuccessful))
            {
                requestSpec.WasSuccessful = false;
                return OrderExecutionResultState.Successful;
            }

            requestSpec.WasSuccessful = true;
            return OrderExecutionResultState.Successful;
        }
        catch (Exception e)
        {
            RequestSpec.AppLogger?.LogError($"{RequestSpec.OrderLoggerId}execution of requeststep failed", e);
            return OrderExecutionResultState.Unsuccessful;
        }
    }

    /// <summary>
    /// Execute the request sending a message to the device and waiting for answers
    /// </summary>
    /// <returns>Execution result</returns>
    /// <remarks>The execution of a request step consists of two major steps. Step 1 is sending a message to the device. Step 2 is waiting for the answer of the device. The device is normally responsive.
    /// A timelag between step1 and step2 may lead to failing device orders due to "lost" device messages. Therefore we start step 2 before step 1 is fired.</remarks>
    public IOrderExecutionResultState ExecuteRequest(IOutboundDataMessage message, IDeviceRequestSpec requestSpec)
    {
        // Set the next request answer step
        requestSpec.CurrentSentMessage = message;

        var s = $"{requestSpec.OrderLoggerId}ExecuteRequest: prepare start";
        Debug.Print($"{s}");
        requestSpec.AppLogger?.LogDebug(s);

        // ******************
        // Start step 2: start the message receiver process on order execution side and wait for incoming messages
        // ******************
        var timeout = CalculateTimeout(requestSpec.Timeout);

        if (IsCancelled)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        _tcs = new CancellationTokenSource(timeout);
        var task = RunStep2Task(this, _tcs);

        if (!IsCancelled)
        {
            return RunStep1(message, requestSpec, task, _tcs);
        }

        _tcs.Dispose();
        Wait(task);
        return OrderExecutionResultState.Unsuccessful;

        // ******************
        // Start step 1: send the message and wait for handshake
        // ******************
    }

    private static IOrderExecutionResultState RunStep1(IOutboundDataMessage message, IDeviceRequestSpec requestSpec,
        Task<IOrderExecutionResultState> task, CancellationTokenSource tcs)
    {
        string s;
        var result = requestSpec.SendDataMessageDelegate?.Invoke(message) ?? MessageSendingResultHelper.Error();
        requestSpec.AppLogger?.LogInformation($"{requestSpec.OrderLoggerId}message sent {requestSpec.CurrentSentMessage?.ToShortInfoString()} with result {result.ProcessExecutionResult}");

        // Handle result from sending
        var execResult = result.ProcessExecutionResult;

        // If the expected handshake for the sent message is NOT correct
        if (!requestSpec.ExpectedHandshakeForSentMessage.Contains(execResult))
        {
            s = $"{requestSpec.OrderLoggerId}sending message ended with unexpected handshake {execResult} {result.Message}"
                .TrimEnd();
            Debug.Print(s);
            requestSpec.AppLogger?.LogDebug(s);

            //CancelTask(task);
            tcs.Dispose();
            Wait(task);
            return result.ProcessExecutionResult;
        }

        // A NACK was received
        if (requestSpec.RequestRequiresOnlyAHandshakeAsAnswer && execResult == OrderExecutionResultState.Nack)
        {
            //CancelTask(task);
            tcs.Dispose();
            Wait(task);
            return HandleNack(requestSpec, message, execResult);
        }

        // The requested handshake was received
        s = $"{requestSpec.OrderLoggerId}sent message {message.ToInfoString()}";
        Debug.Print(s);
        requestSpec.AppLogger?.LogDebug(s);

        var result2 = Wait(task);

        s = $"{requestSpec.OrderLoggerId}wait for answer done: {result2}";
        Debug.Print(s);
        requestSpec.AppLogger?.LogDebug(s);

        return result2;
    }

    private static Task<IOrderExecutionResultState> RunStep2Task(DeviceRequestStepProcessor processor, CancellationTokenSource tcs)
    {
        return Task.Run(() =>
        {
            try
            {
                //Debug.Print("Start waiting for answer");
                processor.ProcessChain();

                Debug.Print($"RSP: left chain processing: {processor.Result}");
                return processor.Result;
            }
            catch (Exception e)
            {
                processor.RequestSpec.AppLogger?.LogError($"{processor.RequestSpec.OrderLoggerId}processing chain failed", e);
                return OrderExecutionResultState.Unsuccessful;
            }
        }, tcs.Token);
    }

    private static int CalculateTimeout(int timeout)
    {
        if (timeout < int.MaxValue - 100)
        {
            timeout += 100;
        }
        else
        {
            timeout = int.MaxValue;
        }

        return timeout;
    }

    private static IOrderExecutionResultState Wait(Task<IOrderExecutionResultState> task)
    {
        try
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                return OrderExecutionResultState.Unsuccessful;
            }
            task.Wait();
            return task.Result;
        }
        catch
        {
            return OrderExecutionResultState.Unsuccessful;
        }
    }

    //private void CancelTask(Task task)
    //{
    //    try
    //    {
    //        if (task.IsCompleted || task.IsCanceled || task.IsCompletedSuccessfully || task.IsFaulted)
    //        {
    //            _token.Dispose();
    //            return;
    //        }

    //        _token.Cancel(true);
    //        _token.Dispose();
    //    }
    //    catch
    //    {
    //        // Do nothing
    //    }
    //}

    private static IOrderExecutionResultState HandleNack(IDeviceRequestSpec requestSpec, IOutboundDataMessage message, IOrderExecutionResultState execResult)
    {
        string s;
        if (execResult.Id != OrderExecutionResultState.Successful.Id)
        {
            s = $"{requestSpec.OrderLoggerId}sending: NACK / CAN handled as success for command {message.ToInfoString()}";
            Debug.Print(s);
            requestSpec.AppLogger?.LogDebug(s);
        }

        var step = requestSpec.RequestAnswerSteps[0];

        step.WasSuccessful = true;

        // Check if the first answer has a delegate to run on success
        var order = requestSpec.ParameterSet?.CurrentOrder;

        if (order == null)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        if (order.IsCancelled)
        {
            return order.ExecutionResult;
        }

        var run = step.AllowedRequestAnswers[0].HandleRequestAnswerOnSuccessDelegate;

        if (run == null)
        {
            execResult = OrderExecutionResultState.Successful;
        }
        else
        {
            var r = run.Invoke(null, null, requestSpec.ParameterSet);

            execResult = r.ExecutionResult;
        }

        s = $"{requestSpec.OrderLoggerId}sending: NACK / CAN handled as success for command {message.ToInfoString()}";
        Debug.Print(s);

        return execResult;
    }

    /// <summary>
    ///  Proces sthe defined chain
    /// </summary>
    public void ProcessChain()
    {
        var requestSpec = RequestSpec;

        CurrentChainElement = (IDeviceRequestAnswerStep)DeviceRequestSpec.RequestAnswerSteps[0];

        Debug.Print($"RSP {requestSpec.GetType().Name}: chain started");
        while (true)
        {
            var element = CurrentChainElement;

            // Run actions before step is processed
            StepHasChanged(CurrentChainElement, requestSpec);

            // Now process the step
            element.ProcessChainElement();

            // An error has happend: leave chain here
            if (!Result.Equals(OrderExecutionResultState.Successful))
            {
                // Call a delegate for failed steps now if available
                element.HandleRequestAnswerStepFailedDelegate?.Invoke();

                Debug.Print($"RSP {requestSpec.GetType().Name}: chain done: Result {Result}");
                break;
            }

            // Last chain element and success
            if (element.NextChainElement == null)
            {
                Debug.Print($"RSP {requestSpec.GetType().Name}: chain done");
                break;
            }

            // More chain elements existing
            Debug.Print($"RSP {requestSpec.GetType().Name}: next element in chain");
            CurrentChainElement = element.NextChainElement;
            Result = OrderExecutionResultState.Unsuccessful;
        }
    }

    private static void StepHasChanged(IRequestAnswerStep step, IRequestSpec requestSpec)
    {
        // Call action before executing if necessary
        step.BeforeExecuteActionDelegate?.Invoke(requestSpec.TransportObject,
            requestSpec.ParameterSet);

        // If no state notication has to be sent before running the step leave here
        if (step.StateToNotifyBeforeRunning == DefaultDeviceStates.DeviceStateOffline)
        {
            return;
        }

        // If an app notification is required before running the current step
        var s = $"{requestSpec.OrderLoggerId}RequestAnswerStep {step}: send state {step.StateToNotifyBeforeRunning} to app before sending message";
        requestSpec.AppLogger?.LogDebug(s);
        // ToDo: needed??
        //requestSpec.DoNotifyDelegate?.Invoke(step.StateToNotifyBeforeRunning);
    }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public bool CheckReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        var requestSpec = RequestSpec;

        var errors = new List<string>();

        //if (requestSpec == null)
        //{
        //    errors.Add("No RequestSpec");
        //    LogErrors(errors, null);
        //    return false;
        //}

        if (IsCancelled)
        {
            errors.Add("Request step processor was cancelled");
            LogErrors(errors, requestSpec);
            return false;
        }

        // internal request
        if (receivedMessage == null)
        {
            return true;
        }

        var chain = CurrentChainElement;

        if (chain == null)
        {
            errors.Add("No chain element loaded to process");
            LogErrors(errors, requestSpec);
            return false;
        }

        // Chain element was already handled
        if (chain.WasSuccessful && CurrentNumberOfMessagesSent == NumberOfMessagesToBeSent)
        {
            errors.Add("Chain element was already processed");
            LogErrors(errors, requestSpec);
            return false;
        }

        // device request
        errors.AddRange(chain.CheckReceivedMessage(receivedMessage));

        if (IsCancelled)
        {
            errors.Add("Request step processor was cancelled");
            LogErrors(errors, requestSpec);
            return false;
        }

        if (Result.Id == OrderExecutionResultState.Successful.Id)
        {
            return true;
        }

        LogErrors(errors, requestSpec);
        return false;

    }

    private static void LogErrors(IList<string> errors, IRequestSpec requestSpec)
    {
        if (!errors.Any())
        {
            return;
        }

        //var localRequestSpec = requestSpec;

        var errorMsg = errors.Aggregate("", (current, msg) => $"{current}{msg}\r\n");
        Debug.Print(errorMsg);

        //if (localRequestSpec == null)
        //{
        //    requestSpec.AppLogger.LogInformation($"{requestSpec.OrderLoggerId}CheckReceivedMessage failed: {errorMsg}");
        //    return;
        //}

        requestSpec.AppLogger?.LogInformation($"{requestSpec.OrderLoggerId}CheckReceivedMessage: {errorMsg}");
    }

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    public void Cancel()
    {
        Result = OrderExecutionResultState.Unsuccessful;
        IsCancelled = true;

        CurrentChainElement?.Cancel();

        var steps = DeviceRequestSpec.RequestAnswerSteps;

        //if (steps == null)
        //{
        //    return;
        //}

        foreach (var x in steps.Where(x => x.WasSuccessful))
        {
            x.Cancel();
        }

        if (_tcs is { IsCancellationRequested: false })
        {
            _tcs?.Cancel(true);
        }

    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        CurrentChainElement = null;
    }
}