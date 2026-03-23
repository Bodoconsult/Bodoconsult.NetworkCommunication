// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class ClientInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    public ClientInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.GetConfig, CreateGetConfigDataMessage);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, CreateStartStreamingDataMessage);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, CreateStopStreamingDataMessage);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, CreateStartSnapshotDataMessage);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, CreateStopSnapshotDataMessage);
    }

    private IBusinessTransactionRequestData? CreateStopSnapshotDataMessage(BtcpInboundDataMessage request)
    {
        throw new NotImplementedException();
    }

    private IBusinessTransactionRequestData? CreateStartSnapshotDataMessage(BtcpInboundDataMessage request)
    {
        throw new NotImplementedException();
    }

    private IBusinessTransactionRequestData? CreateStopStreamingDataMessage(BtcpInboundDataMessage request)
    {
        throw new NotImplementedException();
    }

    private IBusinessTransactionRequestData? CreateStartStreamingDataMessage(BtcpInboundDataMessage request)
    {
        throw new NotImplementedException();
    }

    private IBusinessTransactionRequestData? CreateGetConfigDataMessage(BtcpInboundDataMessage request)
    {
        throw new NotImplementedException();
    }

}