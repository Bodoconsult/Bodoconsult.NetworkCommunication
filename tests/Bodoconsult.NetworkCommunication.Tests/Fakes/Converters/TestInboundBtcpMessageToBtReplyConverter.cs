// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

namespace Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;

public class TestInboundBtcpMessageToBtReplyConverter : BaseInboundBtcpMessageToBtReplyConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    public TestInboundBtcpMessageToBtReplyConverter(IAppLoggerProxy appLogger) : base(appLogger)
    {
    }
}