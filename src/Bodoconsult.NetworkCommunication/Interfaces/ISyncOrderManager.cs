// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing the management of sync running orders. As long as the order IDs are unique instance can be singleton
/// </summary>
public interface ISyncOrderManager : IDisposable
{
    /// <summary>
    /// Is queue with the sync running orders empty
    /// </summary>
    bool IsSyncRunningOrderEmpty { get; }

    /// <summary>
    /// Add a order to the sync execution queue
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <param name="timeout">Timeout in ms</param>
    SyncExecutionData AddSyncExecutionOrder(long orderId, int timeout);

    /// <summary>
    /// Remove a sync execution order from  sync execution queue
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    void RemoveSyncExecutionOrder(long orderId);

    /// <summary>
    /// Get the sync running execution data for a order
    /// </summary>
    /// <param name="orderId">ID of the order</param>
    /// <returns>Sync running execution data or null</returns>
    SyncExecutionData GetSyncExecutionDataForOrder(long orderId);
}