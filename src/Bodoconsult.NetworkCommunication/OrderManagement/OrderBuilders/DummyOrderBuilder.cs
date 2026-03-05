// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create a dummy order waiting for an answer
/// </summary>
public class DummyOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public DummyOrderBuilder() : base(typeof(EmptyParameterSet), BuiltinOrders.DummyOrder)
    { }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        // Tracing
        order.TraceCodeSuccess = TraceCodes.IdsMsgDummyOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgDummyOrderFails;
        order.TraceMessage = OrderTypeName;
    }
}