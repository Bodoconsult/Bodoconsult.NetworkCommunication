// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for <see cref="IRequestStepProcessorFactory"/> instances per tower
/// </summary>
public interface IRequestStepProcessorFactoryFactory
{
    /// <summary>
    /// Create a <see cref="IRequestStepProcessorFactory"/> instance per tower
    /// </summary>
    /// <param name="communicationAdapter">Current communication adapter to use</param>
    /// <returns>A <see cref="IRequestStepProcessorFactory"/> instance</returns>
    IRequestStepProcessorFactory CreateProcessorStepFactory(IOrderManagementCommunicationAdapter communicationAdapter);

}