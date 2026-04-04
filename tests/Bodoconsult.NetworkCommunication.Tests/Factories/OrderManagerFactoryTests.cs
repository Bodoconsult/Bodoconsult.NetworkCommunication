// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;


namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class OrderManagerFactoryTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IAppDateService _dateService = TestDataHelper.AppDateService;
    private readonly SyncOrderManager _syncOrderManager = new();
    private readonly IOrderManagementClientNotificationManager _centralClientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appBenchProxy.Dispose();
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IOrderReceiverFactory orderReceiverFactory = new OrderReceiverFactory();

        var orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _centralClientNotificationManager, _appBenchProxy);
        IRequestProcessorFactoryFactory requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
        IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderFactory orderFactory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        // Act and assert
        Assert.DoesNotThrow(() =>
        {
            var unused = new OrderManagerFactory(orderProcessorFactory, orderReceiverFactory,
                requestStepProcessorFactoryFactory, requestProcessorFactoryFactory, orderPipelineFactory, orderFactory);
        });
    }

    [Test]
    public void CreateInstance_ValidSetup_ReturnsInstance()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(new FakeIpCommunicationAdapter());

        IOrderReceiverFactory orderReceiverFactory = new OrderReceiverFactory();

        var orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _centralClientNotificationManager, _appBenchProxy);
        IRequestProcessorFactoryFactory requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
        IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderFactory orderFactory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var factory = new OrderManagerFactory(orderProcessorFactory, orderReceiverFactory,
            requestStepProcessorFactoryFactory, requestProcessorFactoryFactory, orderPipelineFactory, orderFactory);

        // Act
        var result = factory.CreateInstance(device);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OrderProcessor, Is.Not.Null);
        Assert.That(result.OrderFactory, Is.Not.Null);
        Assert.That(result.MessagingConfig, Is.Not.Null);
        Assert.That(device.OrderManager, Is.Not.Null);
    }

    [Test]
    public void CreateInstance_NoCommAdapter_ReturnsInstance()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();

        IOrderReceiverFactory orderReceiverFactory = new OrderReceiverFactory();

        var orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _centralClientNotificationManager, _appBenchProxy);
        IRequestProcessorFactoryFactory requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
        IRequestStepProcessorFactoryFactory requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderFactory orderFactory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var factory = new OrderManagerFactory(orderProcessorFactory, orderReceiverFactory,
            requestStepProcessorFactoryFactory, requestProcessorFactoryFactory, orderPipelineFactory, orderFactory);

        // Act
        Assert.Throws<ArgumentNullException>(() =>
        {
            var unused = factory.CreateInstance(device);
        });
    }
}