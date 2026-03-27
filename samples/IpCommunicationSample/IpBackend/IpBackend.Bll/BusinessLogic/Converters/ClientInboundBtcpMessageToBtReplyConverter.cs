// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class ClientInboundBtcpMessageToBtReplyConverter : BaseInboundBtcpMessageToBtReplyConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionReplyDelegates.Add(ServerSideBusinessTransactionIds.StateChangedEventFired, CreateStateChangedEventFiredReply);
    }

    private IBusinessTransactionReply? CreateStateChangedEventFiredReply(BtcpInboundDataMessage request)
    {


        return CreateDefaultReply(request);
    }

    protected IBusinessTransactionReply? CreateDefaultReply(BtcpInboundDataMessage request)
    {
        if (request.IsRequest)
        {
            return null;
        }

        var ir = new DefaultBusinessTransactionReply();
        

        if (request.DataBlock == null)
        {
            return ir;
        }

        var payload = request.DataBlock.Data;
        var payloadString = Encoding.UTF8.GetString(payload.Span);



        return ir;
    }
}