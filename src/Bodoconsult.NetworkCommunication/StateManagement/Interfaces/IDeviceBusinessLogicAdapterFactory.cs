// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Base interface for connecting a comm layer with a device specific business logic
/// </summary>
public interface IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Current device
    /// </summary>
    IDeviceBusinessLogicAdapter CreateInstance(IIpDevice device);
}