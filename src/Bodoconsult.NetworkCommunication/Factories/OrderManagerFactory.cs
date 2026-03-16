// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderManagerFactory"/>
/// </summary>
public class OrderManagerFactory : IOrderManagerFactory
{
    private readonly IOrderProcessorFactory _orderProcessorFactory;
    private readonly IOrderReceiverFactory _orderReceiverFactory;
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory;
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory;
    private readonly IOrderPipelineFactory _orderPipelineFactory;
    private readonly IOrderFactory _orderFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="orderProcessorFactory">Current tower order processor factory</param>
    /// <param name="orderReceiverFactory">Current order receiver factory</param>
    /// <param name="requestStepProcessorFactoryFactory">Current request step processor factory</param>
    /// <param name="requestProcessorFactoryFactory">Current request processor factory</param>
    /// <param name="orderPipelineFactory">Current order pipeline factory</param>
    /// <param name="orderFactory">Current order factory</param>
    public OrderManagerFactory(IOrderProcessorFactory orderProcessorFactory,
        IOrderReceiverFactory orderReceiverFactory,
        IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory,
        IRequestProcessorFactoryFactory requestProcessorFactoryFactory,
        IOrderPipelineFactory orderPipelineFactory,
        IOrderFactory orderFactory)
    {
        _orderProcessorFactory = orderProcessorFactory;
        _orderReceiverFactory = orderReceiverFactory;
        _requestStepProcessorFactoryFactory = requestStepProcessorFactoryFactory;
        _requestProcessorFactoryFactory = requestProcessorFactoryFactory;
        _orderPipelineFactory = orderPipelineFactory;
        _orderFactory = orderFactory;
    }

    /// <summary>
    /// Create an instance of <see cref="IOrderManager"/>
    /// </summary>
    /// <param name="device">Current IP device</param>
    /// <returns>New instance of <see cref="IOrderManager"/></returns>
    public IOrderManager CreateInstance(IOrderManagementDevice device)
    {
        ArgumentNullException.ThrowIfNull(device.CommunicationAdapter);

        var receiver = _orderReceiverFactory.CreateInstance(device.DataMessagingConfig.MonitorLogger);
        var requestStepProcessorFactory = _requestStepProcessorFactoryFactory.CreateProcessorStepFactory(device.CommunicationAdapter);
        var requestProcessorFactory = _requestProcessorFactoryFactory.CreateRequestProcessorFactory(requestStepProcessorFactory, device);
        var orderPipeline = _orderPipelineFactory.CreateInstance(device.DataMessagingConfig.LoggerId, requestProcessorFactory);
        var orderProcessor = _orderProcessorFactory.CreateInstance(device, orderPipeline);

        var manager = new OrderManager(device.DataMessagingConfig, orderProcessor, receiver, _orderFactory);
        device.LoadDeviceOrderManager(manager);
        return manager;
    }
}