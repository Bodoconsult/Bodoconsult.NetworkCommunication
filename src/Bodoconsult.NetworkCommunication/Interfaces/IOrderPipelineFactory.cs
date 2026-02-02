// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Factory for creating <see cref="IOrderPipeline"/>
/// </summary>
public interface IOrderPipelineFactory
{
    /// <summary>
    /// Create a new instance of <see cref="IOrderPipeline"/>
    /// </summary>
    /// <param name="loggerId">Logger ID to identity</param>
    /// <param name="requestProcessorFactory">Current request processor factory</param>
    /// <returns>Instance of <see cref="IOrderPipeline"/> created</returns>
    IOrderPipeline CreateInstance(string loggerId, IRequestProcessorFactory requestProcessorFactory);
}