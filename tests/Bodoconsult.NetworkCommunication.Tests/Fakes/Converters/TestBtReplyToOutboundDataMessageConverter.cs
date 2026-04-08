// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;

/// <summary>
/// Test BTCP comm converter for <see cref="IBusinessTransactionReply"/> instances to <see cref="BtcpRequestOutboundDataMessage"/> instances
/// </summary>
public class TestBtReplyToOutboundDataMessageConverter : BaseBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public TestBtReplyToOutboundDataMessageConverter(IAppLoggerProxy appLogger) : base(appLogger)
    { }
}