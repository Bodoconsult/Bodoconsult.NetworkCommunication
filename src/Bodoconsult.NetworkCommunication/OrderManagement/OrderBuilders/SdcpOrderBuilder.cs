// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a SDCP order waiting for an answer
/// </summary>
public class SdcpOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public SdcpOrderBuilder() : base(typeof(SdcpParameterSet), BuiltinOrders.SdcpOrder)
    { }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        order.TraceCodeSuccess = TraceCodes.IdsMsgSdcpOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgSdcpOrderFails;
        order.TraceMessage = "SDCP outbound order";
    }
}