// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpCommunicationSample.Client.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Client.Bll.BusinessTransactions.Providers;

/// <summary>
/// Impl of <see cref="IBusinessTransactionProvider"/> for UPD messaging
/// </summary>
public class BackendTcpIpBusinessTransactionProvider : IBusinessTransactionProvider
{
    public readonly IBackendTcpIpBusinessLogicAdapter BusinessLogicAdapter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public BackendTcpIpBusinessTransactionProvider(IBackendTcpIpBusinessLogicAdapter businessLogicAdapter)
    {
        BusinessLogicAdapter = businessLogicAdapter;

        // Load transaction delegates now
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, Transaction201_StartStreaming);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, Transaction202_StopStreaming);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, Transaction203_StartSnapshot);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, Transaction204_StopSnapshot);

        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.CreateFftAnalysisReport, Transaction250_CreateFftAnalysisReport);

        CreateBusinessTransactionDelegates.Add(ServerSideBusinessTransactionIds.NotificationFired, Transaction100_Notification);
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
    /// Create transaction 2: stop streaming
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
    /// Create transaction 3: start snapshot
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
    /// Create transaction 4: stop snapshot
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

    /// <summary>
    /// Create transaction 4: stop snapshot
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction100_Notification()
    {
        var transaction = new BusinessTransaction
        {
            Id = ServerSideBusinessTransactionIds.NotificationFired,
            Name = "Notification fired",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.NotificationFired
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    public BusinessTransaction Transaction250_CreateFftAnalysisReport()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.CreateFftAnalysisReport,
            Name = "Create FFT analysis report",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.CreateFftAnalysisReport
        };

        transaction.AllowedRequestDataTypes.Add(nameof(FftReportBusinessTransactionRequestData));

        return transaction;
    }
}