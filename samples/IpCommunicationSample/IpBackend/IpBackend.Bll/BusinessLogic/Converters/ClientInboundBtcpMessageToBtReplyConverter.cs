// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

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
}