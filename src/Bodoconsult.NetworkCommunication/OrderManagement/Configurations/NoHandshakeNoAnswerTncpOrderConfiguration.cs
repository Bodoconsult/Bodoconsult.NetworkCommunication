// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// TNCP order not waiting for handshake nor answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class NoHandshakeNoAnswerTncpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerTncpOrderConfiguration() : base(
        $"{BuiltinOrders.NoHandshakeNoAnswerTncpOrder}Configuration", BuiltinOrders.NoHandshakeNoAnswerTncpOrder, new NoHandshakeNoAnswerTncpOrderBuilder())
    { }
}