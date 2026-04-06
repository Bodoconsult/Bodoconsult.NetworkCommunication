// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpBackend.Bll.Interfaces;

namespace IpBackend.Bll.BusinessTransactions.Providers;

/// <summary>
    /// Impl of <see cref="IBusinessTransactionProvider"/> for IP device TCP/IP messaging
    /// </summary>
    public class IpDeviceTcpIpBusinessTransactionProvider : IBusinessTransactionProvider
{
    public readonly IIpDeviceTcpIpDeviceBusinessLogicAdapter BusinessLogicAdapter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpDeviceTcpIpBusinessTransactionProvider(IIpDeviceTcpIpDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        BusinessLogicAdapter = businessLogicAdapter;

        // Load transaction delegates now
        CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StartStreaming, Transaction1_StartStreaming);
        CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopStreaming, Transaction2_StopStreaming);
        CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StartSnapshot, Transaction3_StartSnapshot);
        CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopSnapshot, Transaction4_StopSnapshot);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

    /// <summary>
    /// Create transaction 1: start streaming
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction1_StartStreaming()
    {
        var transaction = new BusinessTransaction
        {
            Id = BackendBusinessTransactionCodes.StartStreaming,
            Name = "Start streaming",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStartStreamingState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }
    /// <summary>
    /// Create transaction 2: stop streaming
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction2_StopStreaming()
    {
        var transaction = new BusinessTransaction
        {
            Id = BackendBusinessTransactionCodes.StopStreaming,
            Name = "Stop streaming",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStopStreamingState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 3: start snapshot
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction3_StartSnapshot()
    {
        var transaction = new BusinessTransaction
        {
            Id = BackendBusinessTransactionCodes.StartSnapshot,
            Name = "Start snapshot",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStartSnapshotState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 4: stop snapshot
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction4_StopSnapshot()
    {
        var transaction = new BusinessTransaction
        {
            Id = BackendBusinessTransactionCodes.StopSnapshot,
            Name = "Stop snapshot",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStopSnapshotState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }
}