// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpClient.Bll.BusinessTransactions.Converters;

/// <summary>
/// Converter for datablocks to business transaction request data
/// </summary>
public class DataBlockConverter
{
    /// <summary>
    /// Convert datablock to business transaction request data
    /// </summary>
    /// <param name="dataBlock">Datablock to convert</param>
    /// <returns>Request data or null if not convertable</returns>
    public IBusinessTransactionRequestData? ConvertToRequest(IInboundDataBlock dataBlock)
    {
        if (dataBlock.Data[..1].Span[0] == 0x73)
        {
            return ConvertToStateStateChangedEventFired(dataBlock.Data[1..].Span);
        }

        return null;
    }


    private IBusinessTransactionRequestData ConvertToStateStateChangedEventFired(Span<byte> span)
    {
        var rd = new StateChangedEventFiredBusinessTransactionRequestData();

        var payload = Encoding.UTF8.GetString(span);

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

//using System.Text;
//using Bodoconsult.App.Abstractions.Interfaces;
//using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
//using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
//using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
//using IpCommunicationSample.Common;
//using IpCommunicationSample.Common.BusinessTransactions;

//namespace IpClient.Bll.BusinessTransactions.Converters;

//public class ClientInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
//{
//    public ClientInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
//    {
//        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.NotificationFired, CreateStateChangedEventFiredBusinessTransaction);
//    }

//    private IBusinessTransactionRequestData? CreateStateChangedEventFiredBusinessTransaction(
//        BtcpRequestInboundDataMessage request)
//    {
//        ArgumentNullException.ThrowIfNull(request.DataBlock);

//        if (request.DataBlock.DataBlockType != DataBlockTypes.NotificationFiredBusiness)
//        {
//            throw new ArgumentException($"Datablock type expected is '{DataBlockTypes.NotificationFiredBusiness}' but was '{request.DataBlock.DataBlockType}'!");
//        }

//        var rd = new StateChangedEventFiredBusinessTransactionRequestData();

//        var payload = Encoding.UTF8.GetString(request.DataBlock.Data.Span);

//        var tokens = payload.Split('\u0005');

//        if (tokens.Length != 6)
//        {
//            throw new ArgumentException("Payload invalid: does not have 6 tokens");
//        }

//        rd.DeviceStateId = Convert.ToInt32(tokens[0]);
//        rd.DeviceStateName = tokens[1];

//        rd.BusinessStateId = Convert.ToInt32(tokens[2]);
//        rd.BusinessStateName = tokens[3];

//        rd.BusinessSubstateId = Convert.ToInt32(tokens[4]);
//        rd.BusinessSubstateName = tokens[5];

//        return rd;
//    }
//}


