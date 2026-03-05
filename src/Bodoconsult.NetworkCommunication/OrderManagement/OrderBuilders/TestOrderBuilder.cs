// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a test order waiting for an answer. Accepts any datablock type
/// </summary>
public class TestOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public TestOrderBuilder() : base(null, BuiltinOrders.TestOrder)
    { }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgTestOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgTestOrderFails;
        order.TraceMessage = OrderTypeName;
    }
}