// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;

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
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, Transaction201_StartStreaming);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, Transaction202_StopStreaming);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, Transaction203_StartSnapshot);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, Transaction204_StopSnapshot);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

    /// <summary>
    /// Create transaction 201: start streaming
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction201_StartStreaming()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StartStreaming,
            Name = "Start streaming",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStartStreamingState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 202: stop streaming
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction202_StopStreaming()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StopStreaming,
            Name = "Stop streaming",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStopStreamingState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 203: start snapshot
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction203_StartSnapshot()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StartSnapshot,
            Name = "Start snapshot",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStartSnapshotState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 204: stop snapshot
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction204_StopSnapshot()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StopSnapshot,
            Name = "Stop snapshot",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStopSnapshotState
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }
}