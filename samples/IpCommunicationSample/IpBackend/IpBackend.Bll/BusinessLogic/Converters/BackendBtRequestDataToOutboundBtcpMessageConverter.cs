// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class BackendBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    public BackendBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy loggerProxy) : base(loggerProxy)
    {
        AllBusinessTransactionRequestDataDelegates.Add(ServerSideBusinessTransactionIds.StateChangedEventFired, CreateStateChangedEventFiredDataMessage);
    }

    private IOutboundBusinessTransactionDataMessage CreateStateChangedEventFiredDataMessage(IBusinessTransactionRequestData request)
    {
        throw new NotImplementedException();
    }
}