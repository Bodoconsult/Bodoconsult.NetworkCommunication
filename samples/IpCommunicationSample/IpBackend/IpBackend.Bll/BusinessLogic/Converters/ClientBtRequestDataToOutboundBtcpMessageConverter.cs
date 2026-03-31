// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

public class ClientBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
    }

    // No requests to handle here

    // Notfications are not handled here. See
}