// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.SyncProcessManager;

/// <summary>
/// Current implementation of <see cref="ISyncProcessManager&lt;T&gt;"/>
/// </summary>
public class SyncProcessManager<T> : ISyncProcessManager<T> where T : class
{
    /// <summary>
    /// The current execution list of sync running orders.
    /// Do not access SyncExecutionQueue directly.
    /// Always take a "copy" of the list with i.e. _syncExecutionQueue.Select or _syncExecutionQueue.ToList to avoid multithreading iusses
    /// </summary>
    private readonly ConcurrentDictionary<Guid, SyncProcessData<T>> _syncExecutionQueue = new();

    /// <summary>
    /// Is queue with the sync running orders empty
    /// </summary>
    public bool IsSyncRunningOrderEmpty => _syncExecutionQueue.IsEmpty;


    /// <summary>
    /// Add an order to the sync execution queue
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    /// <param name="timeout">Timeout in ms</param>
    public SyncProcessData<T> AddSyncProcess(Guid processId, int timeout)
    {
        var syncData = new SyncProcessData<T>(processId, timeout);
        _syncExecutionQueue.TryAdd(processId, syncData);
        return syncData;
    }


    /// <summary>
    /// Remove a sync execution order from  sync execution queue
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    public void RemoveSyncProcess(Guid processId)
    {
        _syncExecutionQueue.TryRemove(processId, out var syncData);
        syncData?.Dispose();
    }


    /// <summary>
    /// Get the sync running execution data for an order
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    /// <returns>Sync running execution data or null</returns>
    public SyncProcessData<T>? GetSyncProcessDataForProcess(Guid processId)
    {
        var success = _syncExecutionQueue.TryRemove(processId, out var syncData);
        return !success ? null : syncData;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _syncExecutionQueue.Clear();
    }
}