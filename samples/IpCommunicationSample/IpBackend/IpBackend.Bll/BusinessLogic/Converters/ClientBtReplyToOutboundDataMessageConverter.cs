// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

/// <summary>
/// Client BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpOutboundDataMessage"/> instances
/// </summary>
public class ClientBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public ClientBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
        
    }


}