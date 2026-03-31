// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSample.Client.Bll.BusinessLogic.Converters;

public class BackendInboundBtcpMessageToBtReplyConverter : BaseInboundBtcpMessageToBtReplyConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public BackendInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        AllBusinessTransactionReplyDelegates.Add(ServerSideBusinessTransactionIds.NotificationFired, CreateDefaultReply);
    }
}