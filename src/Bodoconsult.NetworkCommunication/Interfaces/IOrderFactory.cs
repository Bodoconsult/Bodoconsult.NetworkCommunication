// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for creating orders sent to an IP device
/// </summary>
public interface IOrderFactory
{
    /// <summary>
    /// Current order ID generator
    /// </summary>
    IOrderIdGenerator OrderIdGenerator { get; }

    /// <summary>
    /// Readonly list with all registered order configurations
    /// </summary>
    List<KeyValuePair<string, IOrderConfiguration>> CurrentConfigurations { get; }

    /// <summary>
    /// Create a device order
    /// </summary>
    /// <param name="configName">Name of the requested configuration</param>
    /// <param name="parameterSet">Parameter set to use for the device order</param>
    /// <returns></returns>
    IOrder CreateOrder(string configName, IParameterSet parameterSet);

    /// <summary>
    /// Get an order configuration i.e. for calling <see cref="CreateParameterSetDelegate"/>
    /// </summary>
    /// <param name="configName">Name of the requested configuration</param>
    /// <returns>Configuration or null if none found</returns>
    IOrderConfiguration? GetConfiguration(string configName);

    /// <summary>
    /// The name of the order type this factory is responsible for
    /// </summary>
    void RegisterConfiguration(IOrderConfiguration config);

}