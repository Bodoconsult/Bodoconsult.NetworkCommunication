// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;

/// <summary>
/// Test BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
/// </summary>
public class TestBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public TestBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    { }

    private IOutboundBusinessTransactionDataMessage CreateFromDefaultBusinessTransactionReply(IBusinessTransactionReply reply)
    {
        if (reply is not DefaultBusinessTransactionReply ir)
        {
            throw new ArgumentException($"request is not {nameof(DefaultBusinessTransactionReply)}");
        }

        var msg = new BtcpRequestOutboundDataMessage(ir.RequestData.TransactionId, ir.RequestData.TransactionGuid)
        {
            IsRequest = false
        };

        return msg;
    }
}