// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;

public class TestInboundBtcpMessageToBtRequestDataConverter : BaseInboundBtcpMessageToBtRequestDataConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public TestInboundBtcpMessageToBtRequestDataConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionRequestDataDelegates.Add(99, CreateGetConfigBusinessTransaction);
    }

    private IBusinessTransactionRequestData? CreateGetConfigBusinessTransaction(BtcpRequestInboundDataMessage request)
    {
        var rd = new EmptyBusinessTransactionRequestData
        {
            TransactionId = request.BusinessTransactionId
        };
        return rd;
    }
}