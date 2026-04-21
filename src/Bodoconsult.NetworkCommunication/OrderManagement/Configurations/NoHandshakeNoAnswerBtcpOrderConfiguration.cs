// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// BTCP order not waiting for handshake nor answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class NoHandshakeNoAnswerBtcpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new NoHandshakeNoAnswerBtcpOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerBtcpOrderConfiguration() : base($"{BuiltinOrders.NoHandshakeNoAnswerBtcpOrder}Configuration", BuiltinOrders.NoHandshakeNoAnswerBtcpOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new NoHandshakeNoAnswerBtcpOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}