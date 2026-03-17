// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// BTCP order config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class BtcpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public BtcpOrderConfiguration() : base($"{BuiltinOrders.BtcpOrder}Configuration", BuiltinOrders.BtcpOrder, new BtcpOrderBuilder())
    { }
}