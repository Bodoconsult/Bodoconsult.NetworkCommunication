// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class ClientBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    public ClientBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.StateChangedEventFired, CreateStateChangedEventFiredBusinessTransaction);
    }

    private IOutboundBusinessTransactionDataMessage CreateStateChangedEventFiredBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not StateChangedEventFiredBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Encoding.UTF8.GetBytes($"{rd.DeviceStateId}\u0005{rd.DeviceStateName}\u0005{rd.BusinessStateId}\u0005{rd.BusinessStateName}\u0005{rd.BusinessSubstateId}\u0005{rd.BusinessSubstateName}")
        };

        var message = new BtcpRequestOutboundDataMessage(rd.TransactionId, rd.TransactionGuid)
        {
            DataBlock = db
        };

        return message;
    }
}