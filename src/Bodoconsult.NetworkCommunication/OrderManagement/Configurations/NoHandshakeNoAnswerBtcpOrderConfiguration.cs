// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// BTCP order not waiting for handshake nor answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class NoHandshakeNoAnswerBtcpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public NoHandshakeNoAnswerBtcpOrderConfiguration() : base($"{BuiltinOrders.NoHandshakeNoAnswerBtcpOrder}Configuration", BuiltinOrders.NoHandshakeNoAnswerBtcpOrder, new NoHandshakeNoAnswerBtcpOrderBuilder())
    { }
}