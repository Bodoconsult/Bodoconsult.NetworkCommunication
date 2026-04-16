// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpBackend.Bll.Interfaces;

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
        //CreateBusinessTransactionDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, Transaction201_StartStreaming);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopStreaming, Transaction202_StopStreaming);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StartSnapshot, Transaction203_StartSnapshot);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopSnapshot, Transaction204_StopSnapshot);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();
}