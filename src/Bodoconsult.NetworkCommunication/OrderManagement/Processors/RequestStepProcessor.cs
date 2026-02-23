// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IRequestStepProcessor"/> for device request steps
/// </summary>
public class RequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Current comm adapter
    /// </summary>
    private readonly IOrderManagementCommunicationAdapter _commAdapter;

    private readonly string _loggerId;

    //private readonly object _currentStepLockObject = new();

    private bool _isCancelled;
    private readonly Lock _isCancelledLockObject = new();

    private CancellationTokenSource _token;

    public const int WaitInterval = 20;

    private IRequestAnswerStep _currentChainElement;
    private readonly Lock _currentChainElementObject = new();

    private readonly IOrderManagementDevice _device;

    /// <summary>
    /// Default ctor
    /// </summary>
    public RequestStepProcessor(IRequestSpec requestSpec, IOrderManagementDevice device)
    {
        RequestSpec = requestSpec;

        _commAdapter = device.CommunicationAdapter;
        AppLogger = device.DataMessagingConfig.AppLogger;
        _loggerId = device.DataMessagingConfig.LoggerId;
        OrderLoggerId = $"{_loggerId}{RequestSpec.ParameterSet.CurrentOrder?.LoggerId}RSP: {RequestSpec.Name} ";
        _device = device;
    }

    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Current order ID for logging
    /// </summary>
    public string OrderLoggerId { get; }

    /// <summary>
    /// The current processed chain element
    /// </summary>
    public IRequestAnswerStep CurrentChainElement
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
    /// Message to send to device
    /// </summary>
    public IOutboundDataMessage SentMessage { get; private set; }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    public int NumberOfMessagesToBeSent { get; private set; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    public int CurrentNumberOfMessagesSent { get; private set;}

    ///// <summary>
    ///// Is the sent message returning a NACK or producing another sending error? Default: false
    ///// </summary>
    //public bool IsMessageSendingErrorOrNack { get; set; }

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
        for (var index = 0; index < RequestSpec.RequestAnswerSteps.Count; index++)
        {
            var step = RequestSpec.RequestAnswerSteps[index];

            if (index <= 0)
            {
                continue;
            }

            var elementBefore = RequestSpec.RequestAnswerSteps[index - 1];
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

            var requestSpec = RequestSpec;


            if (requestSpec == null || IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            var sendCancel = requestSpec.ParameterSet?.SendCancelTodeviceIfUnsuccessful ?? false;

            var repeatCount = 0;

            // Only request sending to device proceed here
            requestSpec.CreateMessagesToSend();


            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
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

            var send = requestSpec.SentMessage.ToList();
            NumberOfMessagesToBeSent = send.Count;

            // Process all messages for the request in the same way
            foreach (var message in send)
            {
                // Repeat loop if necessary if the execution result is NOT successful
                IOrderExecutionResultState result;
                while (true)
                {
                    if (IsCancelled)
                    {
                        return OrderExecutionResultState.Unsuccessful;
                    }

                    repeatCount++;
                    result = ExecuteRequest(message, requestSpec);

                    AppLogger.LogDebug($"{OrderLoggerId}ExecuteRequest: {result} at {repeatCount} try");

                    if (result.Id == OrderExecutionResultState.Successful.Id ||
                        repeatCount >= requestSpec.NumberOfRepeatsInCaseOfNoSuccess)
                    {
                        break;
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
                    _commAdapter.CancelRunningOperation();
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
            AppLogger.LogError($"{OrderLoggerId}execution of requeststep failed", e);
            return OrderExecutionResultState.Unsuccessful;
        }
    }

    /// <summary>
    /// Execute the request sending a message to the device and waiting for answers
    /// </summary>
    /// <returns>Execution result</returns>
    /// <remarks>The execution of a request step consists of two major steps. Step 1 is sending a message to the device. Step 2 is waiting for the answer of the device. The device is normally responsive.
    /// A timelag between step1 and step2 may lead to failing device orders due to "lost" device messages. Therefore we start step 2 before step 1 is fired.</remarks>
    public IOrderExecutionResultState ExecuteRequest(IOutboundDataMessage message, IRequestSpec requestSpec)
    {
        // Set the next request answer step
        SentMessage = message;

        var s = $"{OrderLoggerId}ExecuteRequest: prepare start";
        Debug.Print($"{s}");
        AppLogger.LogDebug(s);

        if (IsCancelled)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        // ******************
        // Start step 2: start the message receiver process on order execution side and wait for incoming messages
        // ******************
        var timeout = RequestSpec.Timeout;

        if (timeout < int.MaxValue - 100)
        {
            timeout += 100;
        }
        else
        {
            timeout = int.MaxValue;
        }

        if (IsCancelled)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        _token = new CancellationTokenSource(timeout);
        var task = Task.Run(() =>
        {
            try
            {
                //Debug.Print("Start waiting for answer");
                ProcessChain();

                Debug.Print($"RSP: left chain processing: {Result}");
                return Result;
            }
            catch (Exception e)
            {
                AppLogger.LogError($"{OrderLoggerId}processing chain failed", e);
                return OrderExecutionResultState.Unsuccessful;
            }
        }, _token.Token);

        if (IsCancelled)
        {
            _token.Dispose();
            Wait(task);
            return OrderExecutionResultState.Unsuccessful;
        }

        // ******************
        // Start step 1: send the message and wait for handshake
        // ******************

        Debug.Print("");
        var result = _commAdapter.SendCommandDataMessage(message);
        AppLogger.LogInformation($"{OrderLoggerId}message sent {SentMessage.ToShortInfoString()} with result {result.ProcessExecutionResult}");

        // Handle result from sending
        var execResult = result.ProcessExecutionResult;

        // If the expected handshake for the sent message is NOT correct
        if (!requestSpec.ExpectedHandshakeForSentMessage.Contains(execResult))
        {
            s = $"{OrderLoggerId}sending message ended with unexpected handshake {execResult} {result.Message}"
                .TrimEnd();
            Debug.Print(s);
            AppLogger.LogDebug(s);

            //CancelTask(task);
            _token.Dispose();
            Wait(task);
            return result.ProcessExecutionResult;
        }

        // A NACK was received
        if (requestSpec.RequestRequiresOnlyAHandshakeAsAnswer && execResult == OrderExecutionResultState.Nack)
        {
            //CancelTask(task);
            _token.Dispose();
            Wait(task);
            return HandleNack(requestSpec, message, execResult);
        }

        // The requested handshake was received
        s = $"{OrderLoggerId}sent message {message.ToInfoString()}";
        Debug.Print(s);
        AppLogger.LogDebug(s);

        var result2 = Wait(task);
            
        s = $"{OrderLoggerId}wait for answer done: {result2}";
        Debug.Print(s);
        AppLogger.LogDebug(s);

        return result2;

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


    private void CancelTask(Task task)
    {
        try
        {
            if (task.IsCompleted || task.IsCanceled || task.IsCompletedSuccessfully || task.IsFaulted)
            {
                _token.Dispose();
                return;
            }

            _token.Cancel(true);
            _token.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }

    private IOrderExecutionResultState HandleNack(IRequestSpec requestSpec, IOutboundDataMessage message, IOrderExecutionResultState execResult)
    {
        string s;
        if (execResult.Id != OrderExecutionResultState.Successful.Id)
        {
            s = $"{OrderLoggerId}sending: NACK / CAN handled as success for command {message.ToInfoString()}";
            Debug.Print(s);
            AppLogger.LogDebug(s);
        }

        var step = requestSpec.RequestAnswerSteps[0];

        step.WasSuccessful = true;

        // Check if the first answer has a delegate to run on success
        var order = requestSpec.ParameterSet.CurrentOrder;

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

        s = $"{OrderLoggerId}sending: NACK / CAN handled as success for command {message.ToInfoString()}";
        Debug.Print(s);

        return execResult;
    }

    public void ProcessChain()
    {
        var requestSpec = RequestSpec;

        CurrentChainElement = RequestSpec.RequestAnswerSteps[0];

        Debug.Print($"RSP {requestSpec.GetType().Name}: chain started");
        while (true)
        {
            var element = CurrentChainElement;

            // Run actions before step is processed
            StepHasChanged(CurrentChainElement);

            // Now process the step
            element.ProcessChainElement();

            // An error has happend: leave chain here
            if (Result.Id != OrderExecutionResultState.Successful.Id)
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

    private void StepHasChanged(IRequestAnswerStep step)
    {
        // Call action before executing if necessary
        step.BeforeExecuteActionDelegate?.Invoke(RequestSpec.TransportObject,
            RequestSpec.ParameterSet);

        // If no state notication has to be sent before running the step leave here
        if (step.StateToNotifyBeforeRunning == DefaultDeviceStates.DeviceStateOffline)
        {
            return;
        }

        // If an app notification is required before running the current step
        var s = $"{_loggerId}{OrderLoggerId}RequestAnswerStep {step}: send state {step.StateToNotifyBeforeRunning} to app before sending message";
        AppLogger.LogDebug(s);
        _device.DoNotify(step.StateToNotifyBeforeRunning);
    }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public bool CheckReceivedMessage(IInboundDataMessage receivedMessage)
    {
        var requestSpec = RequestSpec;

        var errors = new List<string>();

        if (requestSpec == null)
        {
            errors.Add("No RequestSpec");
            LogErrors(errors, null);
            return false;
        }

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
        if (CurrentChainElement.WasSuccessful && CurrentNumberOfMessagesSent == NumberOfMessagesToBeSent)
        {
            errors.Add("Chain element was already processed");
            LogErrors(errors, requestSpec);
            return false;
        }

        // device request
        errors.AddRange(CurrentChainElement.CheckReceivedMessage(receivedMessage));

        if (IsCancelled)
        {
            errors.Add("Request step processor was cancelled");
            LogErrors(errors, requestSpec);
            return false;
        }

        if (Result.Id != OrderExecutionResultState.Successful.Id)
        {
            LogErrors(errors, requestSpec);
            return false;
        }

        return true;
    }

    private void LogErrors(IList<string> errors, IRequestSpec requestSpec)
    {
        if (!errors.Any())
        {
            return;
        }

        var localRequestSpec = requestSpec;

        var errorMsg = errors.Aggregate("", (current, msg) => $"{current}{msg}\r\n");
        Debug.Print(errorMsg);

        if (localRequestSpec == null)
        {
            AppLogger.LogInformation($"{_loggerId}{OrderLoggerId}CheckReceivedMessage failed: {errorMsg}");
            return;
        }

        AppLogger.LogInformation($"{_loggerId}{OrderLoggerId}CheckReceivedMessage: {errorMsg}");
    }

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    public void Cancel()
    {
        Result = OrderExecutionResultState.Unsuccessful;
        IsCancelled = true;

        CurrentChainElement?.Cancel();

        var steps = RequestSpec?.RequestAnswerSteps;

        if (steps == null)
        {
            return;
        }

        foreach (var x in steps.Where(x => x.WasSuccessful))
        {
            x.Cancel();
        }

            
        if (!_token.IsCancellationRequested)
        {
            _token?.Cancel(true);
        }

    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        CurrentChainElement = null;
        RequestSpec = null;
    }
}