// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

namespace IpCommunicationSample.Client.Bll.BusinessLogic.Converters;

public class BackendInboundBtcpMessageToBtReplyConverter : BaseInboundBtcpMessageToBtReplyConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public BackendInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
    }
}