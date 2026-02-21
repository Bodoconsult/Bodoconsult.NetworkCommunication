// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Base implementation of <see cref="ISendPacketProcess"/> for sending a message to the device
/// </summary>
public abstract class BaseSendPacketProcess : ISendPacketProcess, IAsyncDisposable
{
    public IDuplexIo DuplexIo;
    protected TaskCompletionSource<bool> TaskCompletionSource;
    private CancellationTokenSource _ctsMain;

    private readonly object _resultLock = new();
    private IOrderExecutionResultState _processExecutionResult;

    /// <summary>
    /// Timeout for waiting for an ackknowledgement
    /// </summary>
    public int Timeout { get; set; } = 12000;  // 4 * wait state timeout for old devices (12s) plus 4 * 5s waiting interval plus transaction time 5s

    /// <summary>
    /// The time interval in ms between 2 tries to send the message
    /// </summary>
    public int WaitingIntervalBetweenRetries { get; set; } = 5000;

    /// <summary>
    /// The total transaction time for all retries for loading the next try
    /// </summary>
    public int TotalTransactionTime { get; set; } = 5000;

    /// <summary>
    /// Timeout in ms for sending messages to the device
    /// </summary>
    public int SendTimeout { get; set; } = 5000;

    /// <summary>
    /// The total timeout for sending a message and receiving a valid handshake for it (<see cref="ISendPacketProcess.Timeout"/> * NumberOfRetries + <see cref="ISendPacketProcess.WaitingIntervalBetweenRetries"/> * (NumberOfRetries - 1) + <see cref="ISendPacketProcess.TotalTransactionTime"/>
    /// </summary>
    public int TotalTimeout => Timeout * MaxSendAttemptCount +
                               (MaxSendAttemptCount - 1) * WaitingIntervalBetweenRetries +
                               TotalTransactionTime;

    /// <summary>
    /// Load dependencies
    /// </summary>
    /// <param name="duplexIo">Current IO messaging system</param>
    /// <param name="message">Current data message to send to the device</param>
    /// <param name="dataMessagingConfig">Current device</param>
    public void LoadDependencies(IDuplexIo duplexIo, IOutboundDataMessage message, IDataMessagingConfig dataMessagingConfig)
    {
        Message = message;
        DuplexIo = duplexIo;
        ProcessExecutionResult = OrderExecutionResultState.NotProcessed;
        DataMessagingConfig = dataMessagingConfig;
    }

    /// <summary>
    /// Current SMD device
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; set; }

    /// <summary>
    /// Maximum number of send attemps
    /// </summary>
    public int MaxSendAttemptCount { get; set; } = DeviceCommunicationBasics.MaxSendAttemptCount;

    /// <summary>
    /// Is the device communication online?
    /// </summary>
    public bool IsComOnline
    {
        get
        {
            var result = false;
            if (DataMessagingConfig.CheckIfCommunicationIsOnlineDelegate != null)
            {
                result = DataMessagingConfig.CheckIfCommunicationIsOnlineDelegate.Invoke();
            }
            return result;
        }
    }

    /// <summary>
    /// Is the device ready?
    /// </summary>
    public bool IsDeviceReady
    {
        get
        {
            var result = false;
            if (DataMessagingConfig?.CheckIfDeviceIsReadyDelegate != null)
            {
                result = DataMessagingConfig.CheckIfDeviceIsReadyDelegate.Invoke();
            }
            return result;
        }
    }

    /// <summary>
    /// Has the send process finished without timeout
    /// </summary>
    public bool HasFinishedWithoutTimeout
    {
        get
        {
            var result = ProcessExecutionResult.Id == OrderExecutionResultState.Successful.Id ||
                         ProcessExecutionResult.Id == OrderExecutionResultState.Can.Id ||
                         ProcessExecutionResult.Id == OrderExecutionResultState.Nack.Id ||
                         ProcessExecutionResult.Id == OrderExecutionResultState.NoResponseFromDevice.Id;
            return result;
        }
    }

    /// <summary>
    /// The execution result of the send process
    /// </summary>
    public IOrderExecutionResultState ProcessExecutionResult
    {
        get
        {
            lock (_resultLock)
            {
                return _processExecutionResult;
            }
        }
        set
        {
            lock (_resultLock)
            {
                _processExecutionResult = value;
            }
        }
    }

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    public virtual void RegisterWaitState()
    {
        // Do nothing
    }

    /// <summary>
    /// Unregister a wait for ACK state to be handled
    /// </summary>
    public virtual void UnregisterWaitState()
    {
        // Do nothing
    }

    /// <summary>
    /// Terminate the waiting task if all process states are processed
    /// </summary>
    public void ProcessDone()
    {


        // Call Context.ProcessDone() only if result was not TimeOut. Otherwise the repeated sending loop is broken
        if (ProcessExecutionResult == OrderExecutionResultState.Timeout)
        {
            Debug.Print("BSSP: process unsuccessful");
            return;
        }
        Debug.Print("BSSP: process successful");

        if (TaskCompletionSource is not
            {
                Task:
                {
                    IsCompleted: false, IsCanceled: false, IsFaulted: false, IsCompletedSuccessfully: false
                }
            })
        {
            return;
        }
        Debug.Print("BSSP: task completed true");
        TaskCompletionSource?.SetResult(true);
    }

    /// <summary>
    /// Current device data message to send
    /// </summary>
    public IOutboundDataMessage Message { get; set; }

    /// <summary>
    /// Send the message
    /// </summary>
    public bool SendMessage()
    {
        if (IsSocketConnected)
        {
            var task = DuplexIo.SendMessageInternal(Message);
            task.Wait(SendTimeout);

            var result = task.Result;

            if (result == null)
            {
                return false;
            }

            Debug.Print($"BSPP:   send result {result.ProcessExecutionResult} {DateTime.Now:O}");

            if (result.ProcessExecutionResult == OrderExecutionResultState.Successful)
            {
                return true;
            }
        }
        else
        {
            DataMessagingConfig.MonitorLogger?.LogError($"{DataMessagingConfig.LoggerId}socket is not open");
            DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("SendPacketProcess - Send message - socket closed");
        }

        return false;
    }

    /// <summary>
    /// Current send attempts counter
    /// </summary>
    public int CurrentSendAttempsCount { get; set; }

    /// <summary>
    /// Is the socket connected?
    /// </summary>
    /// <returns>True if the socket is connected else false</returns>
    public bool IsSocketConnected => DuplexIo.IsConnectionAlive;


    /// <summary>
    /// Create the cancellation token waiting for all send tries to be done
    /// </summary>
    protected void CreateCancellationToken()
    {
        _ctsMain = new CancellationTokenSource(TotalTimeout);
        _ctsMain.Token.Register(() =>
        {
            var ts = TaskCompletionSource;

            UnregisterWaitState();

            if (ts is not
                {
                    Task:
                    {
                        IsCompleted: false, IsCanceled: false, IsFaulted: false, IsCompletedSuccessfully: false
                    }
                })
            {
                return;
            }

            ts.SetResult(false);
            Debug.Print("BSPP: timeout");
        });
    }

    /// <summary>
    /// Create a waiting task, then wait and at the end return the result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskCompletionSource"><see cref="TaskCompletionSource"/> to be handled by the consumer of the waiting task</param>
    /// <returns>Result of the waiting task</returns>
    public static T CreateWaitingTask<T>(out TaskCompletionSource<T> taskCompletionSource)
    {
        // Now wait
        var result = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        taskCompletionSource = result;

        // Wait
        var taskResult = AsyncHelper.RunSync(() => result.Task);
        taskCompletionSource = null;

        return taskResult;
    }


    /// <summary>
    /// Excute the process
    /// </summary>
    public void Execute()
    {

        Debug.Print($"BSSP: start execution {DateTime.Now:O}");

        //Watch = Stopwatch.StartNew();
        CurrentSendAttempsCount = 0;

        // Now prepare waiting for execution
        CreateCancellationToken();

        // Now execute the state chain and if needed repeat it
        AsyncHelper.FireAndForget(() =>
        {
            try
            {
                ExecuteRepeatLoop();
            }
            catch (Exception e)
            {
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}execution failed", e);
                throw;
            }
        });

        // Now wait for execution success or timeout  (doing it in a non-blocking mannor)
        Debug.Print($"BSSP: start MAIN waiting {DateTime.Now:O}");

        var result = CreateWaitingTask(out TaskCompletionSource);

        _ctsMain?.Dispose();
        _ctsMain = null;
        //Watch = null;
        TaskCompletionSource = null;

        Debug.Print($"BSSP: left MAIN waiting {DateTime.Now:O}");

        if (!result)
        {
            ProcessExecutionResult = OrderExecutionResultState.Timeout;
        }

        // Unregister the wait state under all circumstances
        UnregisterWaitState();

        Debug.Print($"BSSP: execution finished {DateTime.Now:O}");
    }

    /// <summary>
    /// Now execute the state chain and if needed repeat it
    /// </summary>
    private void ExecuteRepeatLoop()
    {
        while (CurrentSendAttempsCount <= MaxSendAttemptCount)
        {
            Debug.Print($"BSPP: current attempt {CurrentSendAttempsCount} at {DateTime.Now:hh:mm:ss}");

            // Finished successful: break
            if (!IsDeviceReady)
            {
                ProcessExecutionResult = OrderExecutionResultState.Timeout;
                break;
            }

            // Execute the state chain now
            var result = ExecuteSendAndWait();

            // If handshake was received successfully leave here
            if (result)
            {
                break;
            }

            CurrentSendAttempsCount++;

            // Retry allowed? Yes
            if (CurrentSendAttempsCount <= MaxSendAttemptCount)
            {
                AsyncHelper.Delay(WaitingIntervalBetweenRetries);
                continue;
            }

            // No
            ProcessExecutionResult = OrderExecutionResultState.Timeout;
            break;
        }
    }

    /// <summary>
    /// Execute the entry state
    /// </summary>
    /// <returns>True if a handshake was received else false</returns>
    protected virtual bool ExecuteSendAndWait()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Reset the class to default values
    /// </summary>
    public virtual void Reset()
    {
        UnregisterWaitState();
        Message = null;
        DuplexIo = null;
        ProcessExecutionResult = OrderExecutionResultState.NotProcessed;
        DataMessagingConfig = null;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
        Message = null;
        DuplexIo = null;
        DataMessagingConfig = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (DuplexIo != null) await DuplexIo.DisposeAsync();
    }
}