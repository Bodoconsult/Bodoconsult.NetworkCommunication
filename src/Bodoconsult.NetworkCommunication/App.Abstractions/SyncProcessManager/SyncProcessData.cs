// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions.SyncProcessManager;

/// <summary>
/// Helper class for running process identified by a GUID in a sync manner
/// </summary>
public class SyncProcessData<T> : IDisposable where T: class
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public SyncProcessData(Guid processId, int timeout)
    {
        ProcessId = processId;

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

            TaskCompletionSource?.SetResult(null);

        });

        CancellationTokenSource = cts;
    }

    /// <summary>
    /// Create a task to wait unitl order finished or timeout
    /// </summary>
    /// <returns>Task to wait for</returns>
    public Task<T?> CreateWaitingTask()
    {
        // Now wait
        TaskCompletionSource = new TaskCompletionSource<T?>(TaskCreationOptions.RunContinuationsAsynchronously);
        return TaskCompletionSource.Task;
    }

    /// <summary>
    /// Process ID
    /// </summary>
    public Guid ProcessId { get; }

    /// <summary>
    /// CancellationTokenSource used for running an order in a sync manner
    /// </summary>
    public CancellationTokenSource? CancellationTokenSource { get; private set; }

    /// <summary>
    /// TaskCompletionSource used for running an order in a sync manner
    /// </summary>
    public TaskCompletionSource<T?>? TaskCompletionSource { get; private set; }

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