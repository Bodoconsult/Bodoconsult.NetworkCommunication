// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="ISyncOrderManager"/>
/// </summary>
public class SyncOrderManager: ISyncOrderManager
{
    /// <summary>
    /// The current execution list of sync running orders.
    /// Do not access SyncExecutionQueue directly.
    /// Always take a "copy" of the list with i.e. _syncExecutionQueue.Select or _syncExecutionQueue.ToList to avoid multithreading iusses
    /// </summary>
    private readonly ConcurrentDictionary<long, SyncExecutionData> _syncExecutionQueue = new();

    /// <summary>
    /// Is queue with the sync running orders empty
    /// </summary>
    public bool IsSyncRunningOrderEmpty => _syncExecutionQueue.IsEmpty;

    /// <summary>
    /// Add a order to the sync execution queue
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <param name="timeout">Timeout in ms</param>
    public SyncExecutionData AddSyncExecutionOrder(long orderId, int timeout)
    {
        var syncData = new SyncExecutionData(orderId, timeout);
        _syncExecutionQueue.TryAdd(orderId, syncData);
        return syncData;
    }

    /// <summary>
    /// Remove a sync execution order from  sync execution queue
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    public void RemoveSyncExecutionOrder(long orderId)
    {
        _syncExecutionQueue.TryRemove(orderId, out var syncData);
        syncData?.Dispose();
    }

    /// <summary>
    /// Get the sync running execution data for a order
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Sync running execution data or null</returns>
    public SyncExecutionData GetSyncExecutionDataForOrder(long orderId)
    {
        var success = _syncExecutionQueue.TryRemove(orderId, out var syncData);
        return !success ? null : syncData;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _syncExecutionQueue.Clear();
    }
}