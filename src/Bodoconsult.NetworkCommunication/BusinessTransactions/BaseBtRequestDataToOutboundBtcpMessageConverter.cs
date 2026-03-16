// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions;

/// <summary>
/// Base class for converters from <see cref="IBusinessTransactionRequestData"/> instances to <see cref="BtcpInboundDataMessage"/> instances
/// </summary>
public abstract class BaseBtRequestDataToOutboundBtcpMessageConverter : IBtRequestDataToOutboundDataMessageConverter
{
    /// <summary>
    /// Delegate for creating <see cref="IBusinessTransactionRequestData"/> instances
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns></returns>
    protected delegate IOutboundBusinessTransactionDataMessage CreateBusinessTransactionRequestDataDelegate(IBusinessTransactionRequestData request);

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
    protected BaseBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy appLogger)
    {
        AppLogger = appLogger;
    }

    /// <summary>
    /// Map  an internal business transaction request to a data messaging request
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Internal business transaction request</returns>
    public IOutboundDataMessage MapToOutboundDataMessage(IBusinessTransactionRequestData request)
    {
        // Now search the correct mapper and run it
        try
        {
            foreach (var kvp in AllBusinessTransactionRequestDataDelegates)
            {
                if (request.TransactionId == kvp.Key)
                {
                    continue;
                }

                var s = new StringBuilder();
                s.Append($"BT {request.TransactionId}");

                // Create the internal request
                var internalRequest = kvp.Value.Invoke(request);

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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        throw new ArgumentException(
            "No outbound emssage created as BT " + request.TransactionId + " is not registered!");
    }
}