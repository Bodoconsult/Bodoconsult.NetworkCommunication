// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories creating <see cref="IOrderManager"/> instances for IP devices
/// </summary>
public interface IOrderManagerFactory
{
    /// <summary>
    /// Create an instance of <see cref="IOrderManager"/>
    /// </summary>
    /// <param name="device">Current IP device</param>
    /// <returns>New instance of <see cref="IOrderManager"/></returns>
    IOrderManager CreateInstance(IOrderManagementDevice device);
}