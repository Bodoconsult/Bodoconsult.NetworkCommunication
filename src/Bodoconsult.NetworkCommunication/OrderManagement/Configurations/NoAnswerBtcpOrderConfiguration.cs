// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// BTCP order not waiting for answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class NoAnswerBtcpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new NoAnswerBtcpOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoAnswerBtcpOrderConfiguration() : base($"{BuiltinOrders.NoAnswerBtcpOrder}Configuration", BuiltinOrders.NoAnswerBtcpOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new NoAnswerBtcpOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}