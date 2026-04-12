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
        CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.SendClientHello, Transaction5_SendClientHello);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();

    /// <summary>
    /// Create transaction 5: send cleint hello
    /// </summary>
    /// <returns>Business transaction</returns>
    public BusinessTransaction Transaction5_SendClientHello()
    {
        var transaction = new BusinessTransaction
        {
            Id = BackendBusinessTransactionCodes.SendClientHello,
            Name = "Send client hello",
            RunBusinessTransactionDelegate = BusinessLogicAdapter.SendClientHello
        };

        transaction.AllowedRequestDataTypes.Add(nameof(EmptyBusinessTransactionRequestData));

        return transaction;
    }
}