// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// TNCP order config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class EdcpClientOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpClientOrderConfiguration() : base($"{BuiltinOrders.EdcpClientOrder}Configuration", BuiltinOrders.EdcpClientOrder, new EdcpClientOrderBuilder())
    { }
}