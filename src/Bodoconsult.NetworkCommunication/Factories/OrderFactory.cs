// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderFactory"/>
/// </summary>
public class OrderFactory : IOrderFactory
{
    private readonly Dictionary<string, IOrderConfiguration> _configurations = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="orderIdGenerator">Current ID generator</param>
    public OrderFactory(IOrderIdGenerator orderIdGenerator)
    {
        OrderIdGenerator = orderIdGenerator;
    }

    public IOrderIdGenerator OrderIdGenerator { get; }

    /// <summary>
    /// Readonly list with all registered order configurations
    /// </summary>
    public List<KeyValuePair<string, IOrderConfiguration>> CurrentConfigurations => _configurations.ToList();

    /// <summary>
    /// Create a device order
    /// </summary>
    /// <param name="configName">Name of the requested configuration</param>
    /// <param name="parameterSet">Parameter set to use for the device order</param>
    /// <returns></returns>
    public IOrder CreateOrder(string configName, IParameterSet parameterSet)
    {
        var config = GetConfiguration(configName);
        
        ArgumentNullException.ThrowIfNull(config, $"No config {configName} registered");

        config.ParameterSet = parameterSet;

        ArgumentNullException.ThrowIfNull(config.ParameterSet);
        //ArgumentNullException.ThrowIfNull(config.Device);

        var id = OrderIdGenerator.NextId();

        var order = config.OrderBuilder.CreateOrder(config, id);

        return order;
    }

    /// <summary>
    /// Get an order configuration i.e. for calling <see cref="CreateParameterSetDelegate"/>
    /// </summary>
    /// <param name="configName">Name of the requested configuration</param>
    /// <returns>Configuration or null if none found</returns>
    public IOrderConfiguration? GetConfiguration(string configName)
    {
        var config = _configurations.GetValueOrDefault(configName);

        if (config == null)
        {
            return null;
        }

        return (IOrderConfiguration)config.Clone();
    }

    /// <summary>
    /// The name of the order type this factory is responsible for
    /// </summary>
    public void RegisterConfiguration(IOrderConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config.CreateParameterSetDelegate);

        _configurations.Add(config.ConfigurationName, config);
    }
}