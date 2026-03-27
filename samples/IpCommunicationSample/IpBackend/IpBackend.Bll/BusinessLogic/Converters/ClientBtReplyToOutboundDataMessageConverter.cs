// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

/// <summary>
/// Client BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpOutboundDataMessage"/> instances
/// </summary>
public class ClientBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionReplyDelegates.Add(nameof(DefaultBusinessTransactionReply), CreateFromDefaultBusinessTransactionReply);
    }

    private IOutboundBusinessTransactionDataMessage CreateFromDefaultBusinessTransactionReply(IBusinessTransactionReply reply)
    {
        if (reply is not DefaultBusinessTransactionReply ir)
        {
            throw new ArgumentException($"request is not {nameof(DefaultBusinessTransactionReply)}");
        }

        var msg = new BtcpOutboundDataMessage(ir.RequestData.TransactionId)
        {
            IsRequest = false
        };

        return msg;
    }
}