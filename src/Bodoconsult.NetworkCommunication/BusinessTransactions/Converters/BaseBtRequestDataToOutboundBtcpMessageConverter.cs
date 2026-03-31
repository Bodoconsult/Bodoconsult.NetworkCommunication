// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Text;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

/// <summary>
/// Base class for converters from <see cref="IBusinessTransactionRequestData"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
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
    protected readonly Dictionary<string, CreateBusinessTransactionRequestDataDelegate> AllBusinessTransactionRequestDataDelegates = new();

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
        AllBusinessTransactionRequestDataDelegates.Add(nameof(EmptyBusinessTransactionRequestData), CreateEmptyBtcpRequestMessage);
        AllBusinessTransactionRequestDataDelegates.Add(nameof(StateChangedEventFiredBusinessTransactionRequestData), CreateStateChangedEventFired);
    }

    private static IOutboundBusinessTransactionDataMessage CreateStateChangedEventFired(IBusinessTransactionRequestData request)
    {
        if (request is not StateChangedEventFiredBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var bytes = Encoding.UTF8.GetBytes($"s{rd.DeviceStateId}\u0005{rd.DeviceStateName}\u0005{rd.BusinessStateId}\u0005{rd.BusinessStateName}\u0005{rd.BusinessSubstateId}\u0005{rd.BusinessSubstateName}");

        var db = new BasicOutboundDatablock
        {
            Data = bytes
        };

        var message = new BtcpRequestOutboundDataMessage(rd.TransactionId, rd.TransactionGuid)
        {
            DataBlock = db
        };

        return message;
    }

    private static IOutboundBusinessTransactionDataMessage CreateEmptyBtcpRequestMessage(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(EmptyBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpRequestOutboundDataMessage(rd.TransactionId, rd.TransactionGuid)
        {
            DataBlock = db
        };

        return message;
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
                if (request.GetType().Name != kvp.Key)
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
            $"No outbound message created as BT {request.TransactionId} is not registered!");
    }
}