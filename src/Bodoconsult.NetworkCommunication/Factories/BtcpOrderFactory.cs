// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderFactory"/> for BTCP based orders
/// </summary>
public class BtcpOrderFactory : OrderFactory
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public BtcpOrderFactory(IOrderIdGenerator orderIdGenerator) : base(orderIdGenerator)
    {
        var config1 = new BtcpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new BtcpParameterSet()
        };
        RegisterConfiguration(config1);

        var config2 = new NoAnswerBtcpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new BtcpParameterSet()
        };
        RegisterConfiguration(config2);

        var config3 = new NoHandshakeNoAnswerBtcpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new BtcpParameterSet()
        };
        RegisterConfiguration(config3);
    }
}