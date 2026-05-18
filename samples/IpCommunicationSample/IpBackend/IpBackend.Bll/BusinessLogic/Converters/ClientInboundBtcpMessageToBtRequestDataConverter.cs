// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using IpCommunicationSample.Common.Extensions;

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
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartMessaging, CreateStartMessagingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopMessaging, CreateStopMessagingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.CreateFftAnalysisReport, CreateFftAnalysisReport);
    }

    private IBusinessTransactionRequestData CreateFftAnalysisReport(BtcpRequestInboundDataMessage request)
    {
        var rd = new FftReportBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };
        return rd;
    }

    private IBusinessTransactionRequestData CreateStopMessagingBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.DataBlock);

        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId,
            TransactionGuid = request.BusinessTransactionUid
        };

        return rd;
    }

    private IBusinessTransactionRequestData CreateStartMessagingBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        ArgumentNullException.ThrowIfNull(request.DataBlock);
        
        var rd = request.DataBlock.Data.ToStartMessagingReportBusinessTransactionRequestData();
        rd.TransactionId = request.BusinessTransactionId;
        rd.TransactionGuid = request.BusinessTransactionUid;

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