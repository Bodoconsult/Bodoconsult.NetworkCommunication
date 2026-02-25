// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for <see cref="IRequestProcessor"/> instances based on <see cref="RequestProcessor"/>
/// </summary>
public class RequestProcessorFactory: IRequestProcessorFactory
{
    private readonly IRequestStepProcessorFactory _requestStepProcessorFactory;
    private readonly IOrderManagementDevice _device;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestStepProcessorFactory">Current request step processor factory</param>
    /// <param name="device">Current order management device</param>
    public RequestProcessorFactory(IRequestStepProcessorFactory requestStepProcessorFactory,
        IOrderManagementDevice device)
    {
        _requestStepProcessorFactory = requestStepProcessorFactory;
        _device=device;
    }

    /// <summary>
    /// Create a tower order
    /// </summary>
    /// <param name="order">Current tower order to process</param>
    /// <returns>A request processor for the order</returns>
    public IRequestProcessor CreateRequestProcessor(IOrder order)
    {
        return new RequestProcessor(order, _requestStepProcessorFactory, _device);
    }
}