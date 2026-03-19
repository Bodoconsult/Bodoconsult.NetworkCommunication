// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderFactory"/> for EDCP client based orders
/// </summary>
public class EdcpClientOrderFactory : OrderFactory
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpClientOrderFactory(IOrderIdGenerator orderIdGenerator) : base(orderIdGenerator)
    {
        var config1 = new EdcpClientOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config1);

        var config2 = new NoAnswerEdcpClientOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config2);

        var config3 = new NoHandshakeNoAnswerEdcpClientOrderConfiguration
        {
            CreateParameterSetDelegate = () => new EdcpParameterSet()
        };
        RegisterConfiguration(config3);
    }
}