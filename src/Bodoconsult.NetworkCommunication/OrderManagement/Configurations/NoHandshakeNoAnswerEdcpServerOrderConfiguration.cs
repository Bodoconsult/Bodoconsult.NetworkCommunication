// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// TNCP order not waiting for handshake nor answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class NoHandshakeNoAnswerEdcpServerOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new NoHandshakeNoAnswerEdcpServerOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerEdcpServerOrderConfiguration() : base(
        $"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Configuration", BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new NoHandshakeNoAnswerEdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}