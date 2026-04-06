// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpClient.Bll.BusinessLogic.Converters;

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
        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.NotificationFired, CreateNotificationFired);
    }

    private IBusinessTransactionRequestData? CreateNotificationFired(BtcpRequestInboundDataMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.DataBlock);

        if (request.DataBlock.DataBlockType != DataBlockTypes.NotificationFiredBusiness)
        {
            throw new ArgumentException($"Datablock type expected is '{DataBlockTypes.NotificationFiredBusiness}' but was '{request.DataBlock.DataBlockType}'!");
        }

        var payloadData = request.DataBlock.Data;

        if (payloadData.Length == 0)
        {
            throw new ArgumentException("No state data provided");
        }

        var payloadType = payloadData[..1].Span[0];

        IBusinessTransactionRequestData? rd = null;

        if (payloadType == 0x73) // s for StateMachineStateNotification
        {
            rd = CreateStateChangedEventFiredBusinessTransactionRequestData(payloadData.Slice(1), request);
        }

        return rd;
    }

    private IBusinessTransactionRequestData CreateStateChangedEventFiredBusinessTransactionRequestData(
        Memory<byte> payloadData, BtcpRequestInboundDataMessage request)
    {
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