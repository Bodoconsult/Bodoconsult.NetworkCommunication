// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

namespace Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;

public class TestBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    /// <param name="appGlobals">Current app globals</param>
    public TestBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy appLogger, IAppGlobals appGlobals) : base(appLogger, appGlobals)
    { }
}