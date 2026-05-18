// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpClient.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpClient.Bll.BusinessTransactions.Providers;

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
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartMessaging, Transaction201_StartMessaging);
        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StopMessaging, Transaction202_StopMessaging);

        CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.CreateFftAnalysisReport, Transaction250_CreateFftAnalysisReport);

        CreateBusinessTransactionDelegates.Add(ServerSideBusinessTransactionIds.NotificationFired, Transaction100_Notification);
    }



    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

    /// <summary>
    /// Create transaction 201: start messaging
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction201_StartMessaging()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StartMessaging,
            Name = "Start messaging",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStartMessagingState

        };

        transaction.AllowedRequestDataTypes.Add(nameof(StartMessagingReportBusinessTransactionRequestData));

        return transaction;
    }
    /// <summary>
    /// Create transaction 202: stop messaging
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction202_StopMessaging()
    {
        var transaction = new BusinessTransaction
        {
            Id = ClientSideBusinessTransactionIds.StopMessaging,
            Name = "Stop messaging",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.RequestDeviceStopMessagingState
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