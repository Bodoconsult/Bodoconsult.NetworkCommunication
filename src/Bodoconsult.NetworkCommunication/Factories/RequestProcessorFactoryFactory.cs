// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for <see cref="IRequestProcessorFactory"/> instances based on <see cref="RequestProcessorFactory"/>
/// </summary>
public class RequestProcessorFactoryFactory: IRequestProcessorFactoryFactory
{
    ///// <summary>
    ///// Default ctor
    ///// </summary>
    //public RequestProcessorFactoryFactory()
    //{
    //}
    
    /// <summary>
    /// Create a tower order request processor factory
    /// </summary>
    /// <param name="requestStepProcessorFactory">Current request step processor factory to use</param>
    /// <param name="device">Current device</param>
    /// <returns>a tower order request processor factory</returns>
    public IRequestProcessorFactory CreateRequestProcessorFactory(IRequestStepProcessorFactory requestStepProcessorFactory, IOrderManagementDevice device)
    {
        return new RequestProcessorFactory(requestStepProcessorFactory, device);
    }
}