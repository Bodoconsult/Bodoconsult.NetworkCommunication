// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factory for device request step processor instances
/// </summary>
public interface IRequestStepProcessorFactory
{
    /// <summary>
    /// Create a device request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <param name="deviceServer">Current device server</param>
    /// <returns>A valid device request step processor instance</returns>
    IRequestStepProcessor CreateProcessor(IRequestSpec requestSpec, IOrderManagementDevice deviceServer);

}