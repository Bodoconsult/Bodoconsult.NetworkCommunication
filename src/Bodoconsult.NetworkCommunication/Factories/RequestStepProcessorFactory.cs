// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for <see cref="DeviceRequestStepProcessor"/> instances
/// </summary>
public class RequestStepProcessorFactory : IRequestStepProcessorFactory
{
    /// <summary>
    /// Create a tower request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid tower request step processor instance</returns>
    public IDeviceRequestStepProcessor CreateDeviceProcessor(IDeviceRequestSpec requestSpec)
    {
        return new DeviceRequestStepProcessor(requestSpec);
    }

    /// <summary>
    /// Create a tower request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid tower request step processor instance</returns>
    public IInternalRequestStepProcessor CreateInternalProcessor(IInternalRequestSpec requestSpec)
    {
            return new InternalRequestStepProcessor(requestSpec);
    }
}