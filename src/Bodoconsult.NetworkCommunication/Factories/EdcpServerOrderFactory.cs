// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderFactory"/> for EDCP server based orders
/// </summary>
public class EdcpServerOrderFactory : OrderFactory
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpServerOrderFactory(IOrderIdGenerator orderIdGenerator) : base(orderIdGenerator)
    {
        var config1 = new EdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config1);

        var config2 = new NoAnswerEdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config2);

        var config3 = new NoHandshakeNoAnswerEdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config3);
    }
}