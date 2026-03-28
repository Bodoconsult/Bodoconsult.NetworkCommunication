// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Client.Bll.BusinessTransactions.Converters;

public class ClientInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    public ClientInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.StateChangedEventFired, CreateStateChangedEventFiredBusinessTransaction);
    }

    private IBusinessTransactionRequestData? CreateStateChangedEventFiredBusinessTransaction(
        BtcpRequestInboundDataMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.DataBlock);

        if (request.DataBlock.DataBlockType != DataBlockTypes.StateChangedEventFiredBusiness)
        {
            throw new ArgumentException($"Datablock type expected is 's' but was '{request.DataBlock.DataBlockType}'!");
        }

        var rd = new StateChangedEventFiredBusinessTransactionRequestData();

        var payload = Encoding.UTF8.GetString(request.DataBlock.Data.Span);

        var tokens = payload.Split('\u0005');

        if (tokens.Length != 6)
        {
            throw new ArgumentException("Payload invalid: does not have 6 tokens");
        }

        rd.DeviceStateId = Convert.ToInt32(tokens[0]);
        rd.DeviceStateName = tokens[1];

        rd.BusinessStateId = Convert.ToInt32(tokens[2]);
        rd.BusinessStateName = tokens[3];

        rd.BusinessSubstateId = Convert.ToInt32(tokens[4]);
        rd.BusinessSubstateName = tokens[5];

        return rd;
    }
}