// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpBackend.Bll.BusinessLogic.Converters;

public class ClientInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.GetConfig, CreateGetConfigBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, CreateStartStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, CreateStopStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, CreateStartSnapshotBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, CreateStopSnapshotBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.CreateFftAnalysisReport, CreateFftAnalysisReport);
    }

    private IBusinessTransactionRequestData? CreateFftAnalysisReport(BtcpRequestInboundDataMessage request)
    {
        var rd = new FftReportBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateStopSnapshotBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateStartSnapshotBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateStopStreamingBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateStartStreamingBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateGetConfigBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }
}