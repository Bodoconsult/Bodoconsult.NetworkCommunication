// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Replies;

namespace IpBackend.Bll.BusinessLogic.Converters;

/// <summary>
/// Client BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
/// </summary>
public class ClientBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionReplyDelegates.Add(nameof(FftReportBusinessTransactionReply), CreateFftReportBusinessTransactionReply);
    }

    private IOutboundBusinessTransactionDataMessage CreateFftReportBusinessTransactionReply(IBusinessTransactionReply reply)
    {
        if (reply is not FftReportBusinessTransactionReply ir)
        {
            throw new ArgumentException($"request is not {nameof(DefaultBusinessTransactionReply)}");
        }

        ArgumentNullException.ThrowIfNull(ir.RequestData);

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