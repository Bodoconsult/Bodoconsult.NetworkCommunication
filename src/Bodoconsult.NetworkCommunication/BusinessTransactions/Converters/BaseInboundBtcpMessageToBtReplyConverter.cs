// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Text;

namespace Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

/// <summary>
/// Base class for converters from <see cref="BtcpRequestInboundDataMessage"/> instances to <see cref="IBusinessTransactionRequestData"/> instances
/// </summary>
public abstract class BaseInboundBtcpMessageToBtReplyConverter : IInboundDataMessageToBtReplyConverter
{
    /// <summary>
    /// Delegate for creating <see cref="IBusinessTransactionRequestData"/> instances
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns></returns>
    protected delegate IBusinessTransactionReply? CreateBusinessTransactionReplyDelegate(BtcpReplyInboundDataMessage request);

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
    protected BaseInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger)
    {
        AppLogger = appLogger;
        AllBusinessTransactionReplyDelegates.Add(nameof(DefaultBusinessTransactionReply), CreateDefaultReply);
    }

    /// <summary>
    /// Map a data messaging request to an internal business transaction request
    /// </summary>
    /// <param name="reply">Current request</param>
    /// <returns>Internal business transaction request</returns>
    public IBusinessTransactionReply? MapToBusinessTransactionReply(IInboundDataMessage reply)
    {
        // Request data is required always!
        if (reply is not BtcpReplyInboundDataMessage btm)
        {
            return null;
        }

        // Now search the correct mapper and run it
        //try
        //{
        foreach (var kvp in AllBusinessTransactionReplyDelegates)
        {
            if (btm.GetType().Name != kvp.Key)
            {
                continue;
            }

            var s = new StringBuilder();
            s.Append($"Reply for BT {btm.BusinessTransactionId}");

            // Create the internal request
            var internalRequest = kvp.Value.Invoke(btm);

            if (internalRequest == null)
            {
                s.Append($" mapping for reply data {kvp.Key} returns null");
                return null;
            }

            // Store transaction iD to request
            internalRequest.RequestData.TransactionId = btm.BusinessTransactionId;

            s.Append($" ({internalRequest.RequestData.TransactionGuid})");

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

    protected static IBusinessTransactionReply CreateDefaultReply(BtcpReplyInboundDataMessage request)
    {

        var ir = new DefaultBusinessTransactionReply();

        if (request.DataBlock == null)
        {
            return ir;
        }

        ir.ErrorCode = request.ErrorCode;
        ir.Message = request.InfoMessage;
        ir.ExceptionMessage = request.ErrorMessage;

        return ir;
    }
}