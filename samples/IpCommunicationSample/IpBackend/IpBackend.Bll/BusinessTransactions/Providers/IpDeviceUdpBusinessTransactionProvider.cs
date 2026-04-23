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
public class IpDeviceUdpBusinessTransactionProvider : IBusinessTransactionProvider
{
    public readonly IIpDeviceUdpDeviceBusinessLogicAdapter BusinessLogicAdapter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpDeviceUdpBusinessTransactionProvider(IIpDeviceUdpDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        BusinessLogicAdapter = businessLogicAdapter;

        // Load transaction delegates now
        CreateBusinessTransactionDelegates.Add(ServerSideBusinessTransactionIds.SendClientHello, Transaction205_SendClientHello);
        CreateBusinessTransactionDelegates.Add(ServerSideBusinessTransactionIds.CheckConnection, Transaction206_CheckConnection);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

    /// <summary>
    /// Create transaction 205: send client hello
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction205_SendClientHello()
    {
        var transaction = new BusinessTransaction
        {
            Id = ServerSideBusinessTransactionIds.SendClientHello,
            Name = "Send client hello",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.SendClientHello
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }

    /// <summary>
    /// Create transaction 206: send client hello
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction206_CheckConnection()
    {
        var transaction = new BusinessTransaction
        {
            Id = ServerSideBusinessTransactionIds.CheckConnection,
            Name = "CheckConnection",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.CheckConnection
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }
}