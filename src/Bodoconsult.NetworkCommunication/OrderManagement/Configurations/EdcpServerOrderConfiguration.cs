// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// EDCP server order config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class EdcpServerOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpServerOrderConfiguration() : base($"{BuiltinOrders.EdcpServerOrder}Configuration", BuiltinOrders.EdcpServerOrder, new EdcpServerOrderBuilder())
    { }
}