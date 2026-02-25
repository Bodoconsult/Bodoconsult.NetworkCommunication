// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IRequestStepProcessorFactoryFactory"/>
/// </summary>
public class RequestStepProcessorFactoryFactory : IRequestStepProcessorFactoryFactory
{
    /// <summary>
    /// Create a <see cref="IRequestStepProcessorFactory"/> instance per tower
    /// </summary>
    /// <param name="communicationAdapter">Current communication adapter to use</param>
    /// <returns>A <see cref="IRequestStepProcessorFactory"/> instance</returns>
    public IRequestStepProcessorFactory CreateProcessorStepFactory(IOrderManagementCommunicationAdapter communicationAdapter)
    {
        return new RequestStepProcessorFactory();
    }
}