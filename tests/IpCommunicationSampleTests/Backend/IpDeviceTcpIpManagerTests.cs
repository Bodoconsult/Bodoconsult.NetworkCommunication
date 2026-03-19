// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.Communication.IpDeviceTcpIp;
using IpCommunicationSampleTests.App;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace IpCommunicationSampleTests.Backend;

[TestFixture]
internal class IpDeviceTcpIpManagerTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IAppDateService _dateService = TestDataHelper.AppDateService;
    private readonly SyncOrderManager _syncOrderManager = new();
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();
    private readonly IOrderReceiverFactory _orderReceiverFactory = new OrderReceiverFactory();
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
    private readonly IOrderFactory _orderFactory = new OrderFactory();
    private readonly FakeSendPacketProcessFactory _sendPacketProcessFactory = new();
    private readonly MonitorLoggerFactoryFactory _monitorLoggerFactoryFactory = new(Globals.Instance);
    private readonly LogDataFactory _logDataFactory = TestDataHelper.LogDataFactory;
    private readonly AppLoggerProxyFactory _appLoggerFactory = new();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();
    private readonly TcpIpListenerManager _tcpIpListenerManager = new();

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
        var orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory, _requestProcessorFactoryFactory, orderPipelineFactory, _orderFactory);

        // Act
        var m = new IpDeviceTcpIpManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, 
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger, orderManagerFactory);

        // Assert
        Assert.That(m.Device, Is.Null);
        Assert.That(m.DeviceStateManager, Is.Null);
    }

    [Test]
    public void ConfigureDevice_ValidSetup_DeviceConfigured()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);
        var orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IOrderManagerFactory orderManagerFactory = new OrderManagerFactory(orderProcessorFactory, _orderReceiverFactory, _requestStepProcessorFactoryFactory, _requestProcessorFactoryFactory, orderPipelineFactory, _orderFactory);

        var m = new IpDeviceTcpIpManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger, orderManagerFactory);

        const string ip = "127.0.0.1";
        const int port = 9000;

        // Act  
        m.ConfigureDevice(ip, port);

        // Assert
        Assert.That(m.Device, Is.Not.Null);
        Assert.That(m.DeviceStateManager, Is.Not.Null);
    }
}