// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions.BusinessTransactions;

/// <summary>
/// A fake implementation for testing BT based business logic
/// </summary>
public class FakeBusinessTransactionManager: IBusinessTransactionManager
{
    /// <summary>
    /// Requsted transaction ID
    /// </summary>
    public int RequestedTransactionId { get; set; }

    /// <summary>
    /// The expected reply for the called business transaction
    /// </summary>
    public IBusinessTransactionReply ExpectedBusinessTransactionReply { get; set; } = new DefaultBusinessTransactionReply();

    /// <summary>
    /// Add the transaction delivered by the provider to an internal storage
    /// </summary>
    /// <param name="provider"></param>
    public void AddProvider(IBusinessTransactionProvider provider)
    {
        // Do nothing
    }

    /// <summary>Check for business transaction and return it</summary>
    /// <param name="transactionId">Requested transaction ID</param>
    /// <returns>Business transaction</returns>
    public BusinessTransaction CheckForBusinessTransaction(int transactionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>Run a business transaction</summary>
    /// <param name="transactionId">ID of the requested transaction</param>
    /// <param name="requestData">Data delivered by the request</param>
    /// <returns></returns>
    public IBusinessTransactionReply RunBusinessTransaction(int transactionId, IBusinessTransactionRequestData requestData)
    {
        RequestedTransactionId = transactionId;
        ExpectedBusinessTransactionReply.RequestData = requestData;
        return ExpectedBusinessTransactionReply;
    }
}