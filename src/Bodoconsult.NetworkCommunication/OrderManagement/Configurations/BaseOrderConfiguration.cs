// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// Base class for order configurations
/// </summary>
public abstract class BaseOrderConfiguration: IOrderConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="configurationName">Name of the configuration (for state management etc.)</param>
    /// <param name="orderTypeName">Name of the order type</param>
    /// <param name="orderBuilder">Order builder instance to use for order creation</param>
    protected BaseOrderConfiguration(string configurationName, string orderTypeName,  IOrderBuilder orderBuilder)
    {
        ConfigurationName = configurationName;
        OrderTypeName = orderTypeName;
        OrderBuilder = orderBuilder;
    }

    /// <summary>
    /// Name of the configuration (for state management etc.)
    /// </summary>
    public string ConfigurationName { get; }

    /// <summary>
    /// Name of the order type
    /// </summary>
    public string OrderTypeName { get; }

    /// <summary>
    /// Order builder instance to use for order creation
    /// </summary>
    public IOrderBuilder OrderBuilder { get; }

    ///// <summary>
    ///// The device to run the order
    ///// </summary>
    //public IOrderManagementDevice? Device { get; set; }

    /// <summary>
    /// Current parameterset
    /// </summary>
    public IParameterSet? ParameterSet { get; set; }

    /// <summary>
    /// Current order ID
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Delegate for creating preconfigured parametersets
    /// </summary>
    public CreateParameterSetDelegate? CreateParameterSetDelegate { get; set; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public virtual object Clone()
    {
        throw new NotSupportedException("Override in derived classes");
    }
}