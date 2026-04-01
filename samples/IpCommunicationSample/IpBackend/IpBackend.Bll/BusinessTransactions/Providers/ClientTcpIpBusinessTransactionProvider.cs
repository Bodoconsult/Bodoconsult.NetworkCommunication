// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessTransactions.Providers;

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
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StartStreaming,
        //    Transaction1_StartStreaming);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopStreaming,
        //    Transaction2_StopStreaming);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StartSnapshot,
        //    Transaction3_StartSnapshot);
        //CreateBusinessTransactionDelegates.Add(BackendBusinessTransactionCodes.StopSnapshot, Transaction4_StopSnapshot);
    }

    /// <summary>
    /// A dictionary containing delegates for creating business transactions.
    /// The key of the dictionary is the int transaction ID
    /// </summary>
    public Dictionary<int, CreateBusinessTransactionDelegate> CreateBusinessTransactionDelegates { get; } = new();
}