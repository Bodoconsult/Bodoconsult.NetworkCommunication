// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpCommunicationSample.Common.BusinessTransactions;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class ClientInboundBtcpMessageToBtReplyConverter : BaseInboundBtcpMessageToBtReplyConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
    }

    // No answers from client expected
}