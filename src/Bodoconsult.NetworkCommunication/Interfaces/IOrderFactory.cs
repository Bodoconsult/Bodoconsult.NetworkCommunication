// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for tower orders  providing only first level logic for creation of orders
/// NOT bound to business objects outside TOM
/// </summary>
public interface IOrderFactory
{
    /// <summary>
    /// Create a tower order
    /// </summary>
    /// <param name="parameterSet">Parameter set to use for the tower order</param>
    /// <returns></returns>
    IOrder CreateOrder(IParameterSet parameterSet);


    /// <summary>
    /// The name of the order type this factory is responsible for
    /// </summary>
    string OrderTypeName { get; }

}