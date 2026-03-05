// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Order builder to create an empty order waiting for an answer
/// </summary>
public class EmptyOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EmptyOrderBuilder() : base(typeof(EmptyParameterSet), BuiltinOrders.EmptyOrder)
    { }

    /// <summary>
    /// Configure the order
    /// </summary>
    public override void ConfigureOrder(IOrder order)
    {
        order.TraceCodeSuccess = TraceCodes.IdsMsgEmptyOrderOk;
        order.TraceCodeError = TraceCodes.IdsMsgEmptyOrderFails;
        order.TraceMessage = OrderTypeName;
    }
}