// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="INoAnswerDeviceRequestStepProcessor"/> for device request steps not waiting for an answer
/// </summary>
public class NoAnswerDeviceRequestStepProcessor : INoAnswerDeviceRequestStepProcessor
{
    public const int WaitInterval = 20;

    private bool _isCancelled;
    private readonly Lock _isCancelledLockObject = new();
    private CancellationTokenSource? _tcs;

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoAnswerDeviceRequestStepProcessor(INoAnswerDeviceRequestSpec requestSpec)
    {
        RequestSpec = requestSpec;
        NoAnswerDeviceRequestSpec = requestSpec;

        if (RequestSpec.AppLogger == null)
        {
            throw new ArgumentNullException(nameof(RequestSpec.AppLogger));
        }
    }

    /// <summary>
    /// Current device request spec
    /// </summary>
    public INoAnswerDeviceRequestSpec NoAnswerDeviceRequestSpec { get; private set; }

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
    /// Execute the request
    /// </summary>
    /// <returns>Execution result</returns>
    public IOrderExecutionResultState ExecuteRequest()
    {
        try
        {
            // Fetch the request spec here to avoid multithread issues
            var requestSpec = NoAnswerDeviceRequestSpec;

            if (IsCancelled)
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

            requestSpec.WasSuccessful = false;
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
    public IOrderExecutionResultState ExecuteRequest(IOutboundDataMessage message, INoAnswerDeviceRequestSpec requestSpec)
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

        if (!IsCancelled)
        {
            return RunStep1(message, requestSpec, _tcs);
        }

        _tcs.Dispose();

        return OrderExecutionResultState.Unsuccessful;

        // ******************
        // Start step 1: send the message and wait for handshake
        // ******************
    }

    private static IOrderExecutionResultState RunStep1(IOutboundDataMessage message, INoAnswerDeviceRequestSpec requestSpec, CancellationTokenSource tcs)
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
            return result.ProcessExecutionResult;
        }

        // The requested handshake was received
        s = $"{requestSpec.OrderLoggerId}sent message {message.ToInfoString()} and received correct handshake";
        Debug.Print(s);
        requestSpec.AppLogger?.LogDebug(s);

        return result.ProcessExecutionResult;
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

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    public void Cancel()
    {
        Result = OrderExecutionResultState.Unsuccessful;
        IsCancelled = true;

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
        // Do nothing
    }
}