// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;

namespace IpClient.Bll.BusinessLogic.Converters;

public class BackendBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    /// <param name="appGlobals">Current app globals</param>
    public BackendBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy appLogger,IAppGlobals appGlobals) : base(appLogger, appGlobals)
    { }
}