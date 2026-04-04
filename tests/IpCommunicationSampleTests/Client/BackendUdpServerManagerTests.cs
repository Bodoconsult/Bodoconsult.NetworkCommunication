// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Client.Bll.Communication;
using IpCommunicationSampleTests.App;


namespace IpCommunicationSampleTests.Client;

[TestFixture]
internal class BackendTcpIpClientManagerTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IAppDateService _dateService = TestDataHelper.AppDateService;
    private readonly SyncOrderManager _syncOrderManager = new();
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();
    private readonly IOrderReceiverFactory _orderReceiverFactory = new OrderReceiverFactory();
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
    private readonly IOrderFactory _orderFactory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);
    private readonly FakeSendPacketProcessFactory _sendPacketProcessFactory = new();
    private readonly MonitorLoggerFactoryFactory _monitorLoggerFactoryFactory = new(Globals.Instance);
    private readonly LogDataFactory _logDataFactory = TestDataHelper.LogDataFactory;
    private readonly AppLoggerProxyFactory _appLoggerFactory = new();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();
    private readonly IOrderIdGenerator _orderIdGenerator = TestDataHelper.DefaultOrderIdGenerator;

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
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);
        var orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory, _requestProcessorFactoryFactory, orderPipelineFactory, _orderFactory);

        // Act
        var m = new BackendTcpIpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, orderManagerFactory, _orderIdGenerator);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.IpDevice, Is.Null);
            Assert.That(m.DeviceBusinessLogicAdapter, Is.Null);
        }
    }

    [Test]
    public void ConfigureDevice_ValidSetup_DeviceConfigured()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);
        var orderProcessorFactory = new OrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory, _requestProcessorFactoryFactory, orderPipelineFactory, _orderFactory);

        var m = new BackendTcpIpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager,  _appLogger, orderManagerFactory, _orderIdGenerator);

        const string ip = "127.0.0.1";
        const int port = 9000;

        // Act  
        m.ConfigureDevice(ip, port);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.IpDevice, Is.Not.Null);
            Assert.That(m.DeviceBusinessLogicAdapter, Is.Not.Null);
        }
    }
}