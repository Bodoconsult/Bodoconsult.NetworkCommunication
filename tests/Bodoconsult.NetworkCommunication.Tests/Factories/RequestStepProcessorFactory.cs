// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

/// <summary>
/// Factory for <see cref="RequestStepProcessor"/> instances
/// </summary>
public class RequestStepProcessorFactory : IRequestStepProcessorFactory
{
    /// <summary>
    /// Create a tower request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <param name="towerServer">Current tower server</param>
    /// <returns>A valid tower request step processor instance</returns>
    public IRequestStepProcessor CreateProcessor(IRequestSpec requestSpec, IOrderManagementDevice towerServer)
    {
        if (requestSpec.IsInternalRequest)
        {
            return new InternalRequestStepProcessor( requestSpec, towerServer);
        }

        return new RequestStepProcessor(requestSpec, towerServer);
    }
}