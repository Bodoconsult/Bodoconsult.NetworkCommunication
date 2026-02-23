// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for device order request processor factories
/// </summary>
public interface IRequestProcessorFactoryFactory
{
    /// <summary>
    /// Create a device order request processor factory
    /// </summary>
    /// <param name="requestStepProcessorFactory">Current request step processor factory to use</param>
    /// <param name="device">Current order management device</param>
    /// <returns>An order request processor factory</returns>
    IRequestProcessorFactory CreateRequestProcessorFactory(IRequestStepProcessorFactory requestStepProcessorFactory, IOrderManagementDevice device);

}