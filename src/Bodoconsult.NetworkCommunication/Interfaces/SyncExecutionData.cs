// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Helper class for running order in a sync manner
/// </summary>
public class SyncExecutionData : IDisposable
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public SyncExecutionData(long orderId, int timeout)
    {
        OrderId = orderId;

        // Create the CancellationTokenSource to implement timeout for sync running
        var cts = new CancellationTokenSource(timeout + 100);
        cts.Token.Register(() =>
        {

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

            TaskCompletionSource?.SetResult(OrderExecutionResultState.Timeout);

        });

        CancellationTokenSource = cts;
    }

    /// <summary>
    /// Create a task to wait unitl order finished or timeout
    /// </summary>
    /// <returns>Task to wait for</returns>
    public Task<IOrderExecutionResultState> CreateWaitingTask()
    {
        // Now wait
        TaskCompletionSource = new TaskCompletionSource<IOrderExecutionResultState>(TaskCreationOptions.RunContinuationsAsynchronously);
        return TaskCompletionSource.Task;
    }

    /// <summary>
    /// Order ID
    /// </summary>
    public long OrderId { get; }

    /// <summary>
    /// CancellationTokenSource used for running an order in an sync manner
    /// </summary>
    public CancellationTokenSource? CancellationTokenSource { get; private set; }

    /// <summary>
    /// TaskCompletionSource used for running an order in a sync manner
    /// </summary>
    public TaskCompletionSource<IOrderExecutionResultState>? TaskCompletionSource { get; private set; }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        try
        {
            if (CancellationTokenSource != null)
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    CancellationTokenSource.Cancel();
                }

                CancellationTokenSource.Dispose();
            }

            CancellationTokenSource = null;
            TaskCompletionSource = null;
        }
        catch //(Exception e)
        {
            // Do nothing
        }

    }
}