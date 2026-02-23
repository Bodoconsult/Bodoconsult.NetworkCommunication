// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for device order request processors
/// </summary>
public interface IRequestProcessorFactory
{
    /// <summary>
    /// Create a device order
    /// </summary>
    /// <param name="order">Current device order to process</param>
    /// <returns>A request processor for the order</returns>
    IRequestProcessor CreateRequestProcessor(IOrder order);

}