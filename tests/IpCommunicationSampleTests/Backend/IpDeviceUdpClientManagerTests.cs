// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.Communication;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Backend;

[TestFixture]
internal class IpDeviceUdpClientManagerTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    //private readonly IAppDateService _dateService = TestDataHelper.AppDateService;
    //private readonly SyncOrderManager _syncOrderManager = new();
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();
    //private readonly IOrderReceiverFactory _orderReceiverFactory = new OrderReceiverFactory();
    //private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
    //private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
    //private readonly IOrderFactory _orderFactory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);
    private readonly FakeSendPacketProcessFactory _sendPacketProcessFactory = new();
    private readonly MonitorLoggerFactoryFactory _monitorLoggerFactoryFactory = new(Globals.Instance);
    private readonly LogDataFactory _logDataFactory = TestDataHelper.LogDataFactory;
    private readonly AppLoggerProxyFactory _appLoggerFactory = new();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();

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
        //var orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        //IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        // Act
        var m = new IpDeviceUdpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger);

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
        //var orderProcessorFactory = new StateMachineOrderProcessorFactory(_dateService, _syncOrderManager, _clientNotificationManager, _appBenchProxy);
        //IOrderPipelineFactory orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);

        var m = new IpDeviceUdpClientManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger);

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