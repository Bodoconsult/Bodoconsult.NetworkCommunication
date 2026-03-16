// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Client.Bll.BusinessTransactions;

public class ClientBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    public ClientBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.GetConfig, CreateGetConfigBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartStreaming, CreateStartStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopStreaming, CreateStopStreamingBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StartSnapshot, CreateStartSnapshotBusinessTransaction);
        AllBusinessTransactionRequestDataDelegates.Add(ClientSideBusinessTransactionIds.StopSnapshot, CreateStopSnapshotBusinessTransaction);
    }

    private IOutboundBusinessTransactionDataMessage CreateStopSnapshotBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpOutboundDataMessage(rd.TransactionId)
        {
            DataBlock = db,
            IsRequest = true,
        };

        return message;
    }

    private IOutboundBusinessTransactionDataMessage CreateStartSnapshotBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpOutboundDataMessage(rd.TransactionId)
        {
            DataBlock = db,
            IsRequest = true,
        };

        return message;
    }

    private IOutboundBusinessTransactionDataMessage CreateStopStreamingBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpOutboundDataMessage(rd.TransactionId)
        {
            DataBlock = db,
            IsRequest = true,
        };

        return message;
    }

    private IOutboundBusinessTransactionDataMessage CreateGetConfigBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpOutboundDataMessage(rd.TransactionId)
        {
            DataBlock = db,
            IsRequest = true,
        };

        return message;
    }

    private IOutboundBusinessTransactionDataMessage CreateStartStreamingBusinessTransaction(IBusinessTransactionRequestData request)
    {
        if (request is not EmptyBusinessTransactionRequestData rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateChangedEventFiredBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Memory<byte>.Empty
        };

        var message = new BtcpOutboundDataMessage(rd.TransactionId)
        {
            DataBlock = db,
            IsRequest = true,
        };

        return message;
    }
}