// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for tower order request processors
/// </summary>
public interface IRequestProcessorFactory
{
    /// <summary>
    /// Create a tower order
    /// </summary>
    /// <param name="order">Current tower order to process</param>
    /// <returns>A request processor for the order</returns>
    IRequestProcessor CreateRequestProcessor(IOrder order);

}