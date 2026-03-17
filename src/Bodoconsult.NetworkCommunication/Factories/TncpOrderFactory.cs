// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderFactory"/> for TNCP based orders
/// </summary>
public class TncpOrderFactory: OrderFactory
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public TncpOrderFactory()
    {
        var config1 = new TncpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new TncpParameterSet()
        };
        RegisterConfiguration(config1);

        var config2 = new NoAnswerTncpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new TncpParameterSet()
        };
        RegisterConfiguration(config2);

        var config3 = new NoHandshakeNoAnswerTncpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new TncpParameterSet()
        };
        RegisterConfiguration(config3);
    }
}