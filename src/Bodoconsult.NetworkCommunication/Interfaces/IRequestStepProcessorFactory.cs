// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factory for tower request step processor instances
/// </summary>
public interface IRequestStepProcessorFactory
{
    /// <summary>
    /// Create a tower request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <param name="towerServer">Current tower server</param>
    /// <returns>A valid tower request step processor instance</returns>
    IRequestStepProcessor CreateProcessor(IRequestSpec requestSpec, IOrderManagementDevice towerServer);

}