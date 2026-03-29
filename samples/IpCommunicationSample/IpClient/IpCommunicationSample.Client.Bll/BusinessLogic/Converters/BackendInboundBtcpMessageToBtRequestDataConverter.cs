// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using System.Text;

namespace IpCommunicationSample.Client.Bll.BusinessLogic.Converters;

/// <summary>
/// Inbound BTCP data message converter for comm from backend
/// </summary>
public class BackendInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public BackendInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.StateChangedEventFired, CreateStateChangedEventFired);
    }

    private IBusinessTransactionRequestData? CreateStateChangedEventFired(BtcpRequestInboundDataMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.DataBlock);

        if (request.DataBlock.DataBlockType != DataBlockTypes.StateChangedEventFiredBusiness)
        {
            throw new ArgumentException($"Datablock type expected is '{DataBlockTypes.StateChangedEventFiredBusiness}' but was '{request.DataBlock.DataBlockType}'!");
        }

        var payloadData = request.DataBlock.Data;

        if (payloadData.Length == 0)
        {
            throw new ArgumentException("No state data provided");
        }

        var payload = Encoding.UTF8.GetString(payloadData.Span);

        var tokens = payload.Split('\u0005');

        if (tokens.Length < 6)
        {
            throw new ArgumentException("Invalid state data provided");
        }

        var rd = new StateChangedEventFiredBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid,
            DeviceStateId = Convert.ToInt32(tokens[0]),
            DeviceStateName = tokens[1],
            BusinessStateId = Convert.ToInt32(tokens[2]),
            BusinessStateName = tokens[3],
            BusinessSubstateId = Convert.ToInt32(tokens[4]),
            BusinessSubstateName = tokens[5],
        };
        return rd;
    }
}