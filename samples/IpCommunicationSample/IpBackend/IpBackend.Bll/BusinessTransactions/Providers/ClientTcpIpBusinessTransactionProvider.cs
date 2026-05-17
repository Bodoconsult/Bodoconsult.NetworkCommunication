// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpBackend.Bll.BusinessTransactions.Providers;

/// <summary>
/// Impl of <see cref="IBusinessTransactionProvider"/> for client TCP/IP messaging
/// </summary>
public class ClientTcpIpBusinessTransactionProvider : IBusinessTransactionProvider
{
    public readonly IClientTcpIpDeviceBusinessLogicAdapter BusinessLogicAdapter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public ClientTcpIpBusinessTransactionProvider(IClientTcpIpDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        BusinessLogicAdapter = businessLogicAdapter;

        //// Load transaction delegates now
        CreateBusinessTransactionDelegates.Add(ServerSideBusinessTransactionIds.ReportDeviceError, Transaction101_ReportDeviceError);
    }

    /// <summary>
    /// Create transaction 101: report device error
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction101_ReportDeviceError()
    {
        var transaction = new BusinessTransaction
        {
            Id = ServerSideBusinessTransactionIds.ReportDeviceError,
            Name = "Report device error",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.ReportDeviceError
        };

        transaction.AllowedRequestDataTypes.Add(nameof(ErrorBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();
}