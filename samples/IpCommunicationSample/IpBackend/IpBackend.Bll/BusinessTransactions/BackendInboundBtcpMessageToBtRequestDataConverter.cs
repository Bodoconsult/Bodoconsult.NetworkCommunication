// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSample.Backend.Bll.BusinessTransactions;

public class BackendInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    public BackendInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.GetConfig, CreateGetConfigBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, CreateStartStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, CreateStopStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, CreateStartSnapshotBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, CreateStopSnapshotBusinessTransaction);
    }

    private IBusinessTransactionRequestData? CreateStopSnapshotBusinessTransaction(BtcpInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData();
        return rd;
    }

    private IBusinessTransactionRequestData? CreateStartSnapshotBusinessTransaction(BtcpInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData();
        return rd;
    }

    private IBusinessTransactionRequestData? CreateStopStreamingBusinessTransaction(BtcpInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData();
        return rd;
    }

    private IBusinessTransactionRequestData? CreateStartStreamingBusinessTransaction(BtcpInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData();
        return rd;
    }

    private IBusinessTransactionRequestData? CreateGetConfigBusinessTransaction(BtcpInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData();
        return rd;
    }
}