// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for order configurations
/// </summary>
public interface IOrderConfiguration : ICloneable
{
    /// <summary>
    /// Name of the configuration (for state management etc.)
    /// </summary>
    string ConfigurationName { get;  }

    /// <summary>
    /// Name of the order type
    /// </summary>
    string OrderTypeName { get; }

    ///// <summary>
    ///// The device to run the order
    ///// </summary>
    //IOrderManagementDevice? Device { get; set; }

    /// <summary>
    /// Order builder instance to use for order creation
    /// </summary>
    IOrderBuilder OrderBuilder { get; }

    /// <summary>
    /// Current parameterset
    /// </summary>
    IParameterSet? ParameterSet { get; set; }

    /// <summary>
    /// Delegate for creating preconfigured parametersets
    /// </summary>
    CreateParameterSetDelegate? CreateParameterSetDelegate { get; set; }
}