// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderPipelineFactory"/>
/// </summary>
public class OrderPipelineFactory: IOrderPipelineFactory
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IAppLoggerProxy _appLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderPipelineFactory(IDateTimeService dateTimeService, IAppLoggerProxy appLogger)
    {
        _dateTimeService = dateTimeService;
        _appLogger = appLogger;
    }

    /// <summary>
    /// Create a new instance of <see cref="IOrderPipeline"/>
    /// </summary>
    /// <param name="loggerId">Logger ID to identity</param>
    /// <param name="requestProcessorFactory">Current request processor factory</param>
    /// <returns>Instance of <see cref="IOrderPipeline"/> created</returns>
    public IOrderPipeline CreateInstance(string loggerId, IRequestProcessorFactory requestProcessorFactory)
    {
        return new OrderPipeline(_dateTimeService, requestProcessorFactory, _appLogger, loggerId);
    }
}