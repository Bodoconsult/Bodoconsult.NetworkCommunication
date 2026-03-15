// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for order ID generators
/// </summary>
public interface IOrderIdGenerator
{
    /// <summary>
    /// Get the next order ID
    /// </summary>
    /// <returns>Order ID</returns>
    long NextId();
}