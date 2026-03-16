// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions;

/// <summary>
/// Base class for converters from <see cref="BtcpInboundDataMessage"/> instances to <see cref="IBusinessTransactionRequestData"/> instances
/// </summary>
public abstract class BaseInboundBtcpMessageToBtRequestDataConverter : IInboundDataMessageToBtRequestDataConverter
{
    /// <summary>
    /// Delegate for creating <see cref="IBusinessTransactionRequestData"/> instances
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns></returns>
    protected delegate IBusinessTransactionRequestData? CreateBusinessTransactionRequestDataDelegate(BtcpInboundDataMessage request);

    /// <summary>
    /// Collection of all registered business transactions and the <see cref="CreateBusinessTransactionRequestDataDelegate"/> to use for the single business transaction
    /// </summary>
    protected readonly Dictionary<int, CreateBusinessTransactionRequestDataDelegate> AllBusinessTransactionRequestDataDelegates = new();

    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    protected BaseInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy appLogger)
    {
        AppLogger = appLogger;
    }

    /// <summary>
    /// Map a data messaging request to an internal business transaction request
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Internal business transaction request</returns>
    public IBusinessTransactionRequestData? MapToBusinessTransactionRequestData(IInboundDataMessage request)
    {
        // Request data is required always!
        if (request is not BtcpInboundDataMessage btm)
        {
            return null;
        }

        // No request
        if (!btm.IsRequest)
        {
            return null;
        }

        // Now search the correct mapper and run it
        //try
        //{
        foreach (var kvp in AllBusinessTransactionRequestDataDelegates)
        {
            if (btm.BusinessTransactionId==kvp.Key)
            {
                continue;
            }

            var s = new StringBuilder();
            s.Append($"BT {btm.BusinessTransactionId}");

            // Create the internal request
            var internalRequest = kvp.Value.Invoke(btm);

            if (internalRequest == null)
            {
                s.Append($" mapping for request data {kvp.Key} returns null");
                return null;
            }

            // Store transaction iD to request
            internalRequest.TransactionId = btm.BusinessTransactionId;

            s.Append($" ({internalRequest.TransactionGuid})");

            try
            {
                s.Append($" requested: {ObjectHelper.GetObjectPropertiesAsString(internalRequest)}");
            }
            catch //(Exception e)
            {
                s.Append($"Object serialization for logging failed {internalRequest.GetType().Name}");
            }

            AppLogger.LogInformation(s.ToString());

            return internalRequest;

        }
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //    throw;
        //}


        return null;
    }

}