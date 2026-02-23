// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for <see cref="IRequestStepProcessorFactory"/> instances per device
/// </summary>
public interface IRequestStepProcessorFactoryFactory
{
    /// <summary>
    /// Create a <see cref="IRequestStepProcessorFactory"/> instance per device
    /// </summary>
    /// <param name="communicationAdapter">Current communication adapter to use</param>
    /// <returns>A <see cref="IRequestStepProcessorFactory"/> instance</returns>
    IRequestStepProcessorFactory CreateProcessorStepFactory(IOrderManagementCommunicationAdapter communicationAdapter);

}