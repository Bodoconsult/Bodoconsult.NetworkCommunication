// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Text;
using Bodoconsult.NetworkCommunication.App.Abstractions.BusinessTransactions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

/// <summary>
/// Base class for converters from <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
/// </summary>
public abstract class BaseBtReplyToOutboundDataMessageConverter : IBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Delegate for creating <see cref="IBusinessTransactionRequestData"/> instances
    /// </summary>
    /// <param name="reply">Current reply</param>
    /// <returns>Outbound data message</returns>
    protected delegate IOutboundBusinessTransactionDataMessage? CreateBusinessTransactionReplyDelegate(IBusinessTransactionReply reply);

    /// <summary>
    /// Collection of all registered business transactions and the <see cref="CreateBusinessTransactionReplyDelegate"/> to use for the single business transaction
    /// </summary>
    protected readonly Dictionary<string, CreateBusinessTransactionReplyDelegate> AllBusinessTransactionReplyDelegates = new();

    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    protected BaseBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger)
    {
        AppLogger = appLogger;

        AllBusinessTransactionReplyDelegates.Add(nameof(DefaultBusinessTransactionReply), CreateFromDefaultBusinessTransactionReply);
        AllBusinessTransactionReplyDelegates.Add(nameof(DoNotSendBusinessTransactionReply), CreateFromDoNotSendBusinessTransactionReply);
    }

    /// <summary>
    /// Map  an internal business transaction request to a data messaging request
    /// </summary>
    /// <param name="reply">Current request</param>
    /// <returns>Internal business transaction request</returns>
    public IOutboundDataMessage? MapToOutboundDataMessage(IBusinessTransactionReply reply)
    {
        // Now search the correct mapper and run it
        try
        {
            foreach (var kvp in AllBusinessTransactionReplyDelegates)
            {
                if (reply.GetType().Name != kvp.Key)
                {
                    continue;
                }

                var s = new StringBuilder();
                s.Append($"BT {reply.RequestData.TransactionId}");

                // Create the internal request
                var internalRequest = kvp.Value.Invoke(reply);

                if (internalRequest == null)
                {
                    return null;
                }

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

            throw new ArgumentException($"No outbound message created as BT {reply.RequestData.TransactionId} is not registered!");
        }
        catch (Exception e)
        {
            throw new ArgumentException("No outbound message created", e);
        }
    }

    /// <summary>
    /// Create NO outvound message for <see cref="DoNotSendBusinessTransactionReply"/>
    /// </summary>
    /// <param name="reply"><see cref="DoNotSendBusinessTransactionReply"/> instance</param>
    /// <returns>DoNotSendBusinessTransactionReply</returns>
    /// <exception cref="ArgumentException">Not a <see cref="DoNotSendBusinessTransactionReply"/> was provided</exception>
    private static IOutboundBusinessTransactionDataMessage? CreateFromDoNotSendBusinessTransactionReply(IBusinessTransactionReply reply)
    {
        if (reply is not DoNotSendBusinessTransactionReply)
        {
            throw new ArgumentException($"request is not {nameof(DoNotSendBusinessTransactionReply)}");
        }
        return null;
    }

    /// <summary>
    /// Create outbound message for <see cref="DefaultBusinessTransactionReply"/>
    /// </summary>
    /// <param name="reply"><see cref="DefaultBusinessTransactionReply"/> instance</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Not a <see cref="DefaultBusinessTransactionReply"/> was provided</exception>
    private static IOutboundBusinessTransactionDataMessage CreateFromDefaultBusinessTransactionReply(IBusinessTransactionReply reply)
    {
        if (reply is not DefaultBusinessTransactionReply ir)
        {
            throw new ArgumentException($"request is not {nameof(DefaultBusinessTransactionReply)}");
        }

        var payload = $"{ir.ErrorCode}|{ir.Message?.Replace("|", "")}|{ir.ExceptionMessage?.Replace("|", "")}";

        var dataBlock = new BasicOutboundDatablock
        {
            Data = Encoding.UTF8.GetBytes(payload)
        };

        var msg = new BtcpReplyOutboundDataMessage(ir.RequestData.TransactionId, ir.RequestData.TransactionGuid)
        {
            DataBlock = dataBlock,
            ErrorCode = ir.ErrorCode,
            InfoMessage = ir.Message?.Replace("|", ""),
            ErrorMessage = ir.ExceptionMessage?.Replace("|", "")
        };

        return msg;
    }
}