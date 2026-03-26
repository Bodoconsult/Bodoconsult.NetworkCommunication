// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.App.Abstractions.SyncProcessManager;

/// <summary>
/// Interface for implementing the management of sync running non-blocking processes. As long as the process IDs are unique instance can be singleton
/// </summary>
public interface ISyncProcessManager<T> : IDisposable where T: class
{
    /// <summary>
    /// Is queue with the sync running orders empty
    /// </summary>
    bool IsSyncRunningOrderEmpty { get; }

    /// <summary>
    /// Add an order to the sync execution queue
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    /// <param name="timeout">Timeout in ms</param>
    SyncProcessData<T> AddSyncProcess(Guid processId, int timeout);

    /// <summary>
    /// Remove a sync execution order from  sync execution queue
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    void RemoveSyncProcess(Guid processId);

    /// <summary>
    /// Get the sync running execution data for an order
    /// </summary>
    /// <param name="processId">GUID of the rpocess to run sync</param>
    /// <returns>Sync running execution data or null</returns>
    SyncProcessData<T>? GetSyncProcessDataForProcess(Guid processId);
}