// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace IpClient.Bll.BusinessLogic.Converters;

/// <summary>
/// Backend BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
/// </summary>
public class BackendBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public BackendBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    { }
}