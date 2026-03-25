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
using IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;
using IpCommunicationSample.Backend.Bll.Communication;
using IpCommunicationSampleTests.App;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace IpCommunicationSampleTests.Backend;

[TestFixture]
internal class BackendManagerTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IAppDateService _dateService = TestDataHelper.AppDateService;
    private readonly SyncOrderManager _syncOrderManager = new();
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();
    private readonly IOrderReceiverFactory _orderReceiverFactory = new OrderReceiverFactory();
    private readonly IRequestProcessorFactoryFactory _requestProcessorFactoryFactory = new RequestProcessorFactoryFactory();
    private readonly IRequestStepProcessorFactoryFactory _requestStepProcessorFactoryFactory = new RequestStepProcessorFactoryFactory();
    private readonly FakeSendPacketProcessFactory _sendPacketProcessFactory = new();
    private readonly MonitorLoggerFactoryFactory _monitorLoggerFactoryFactory = new(Globals.Instance);
    private readonly LogDataFactory _logDataFactory = TestDataHelper.LogDataFactory;
    private readonly AppLoggerProxyFactory _appLoggerFactory = new();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();
    private readonly TcpIpListenerManager _tcpIpListenerManager = new();
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
        var orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);

        // Act
        var m = new BackendManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _tcpIpListenerManager, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.IpDeviceTcpIp, Is.Null);
            Assert.That(m.IpDeviceUdp, Is.Null);
            Assert.That(m.Client, Is.Null);
            Assert.That(m.IpDeviceTcpIpConfig, Is.Null);
            Assert.That(m.IpDeviceUdpConfig, Is.Null);
            Assert.That(m.ClientTcpIpConfig, Is.Null);
        }
    }


}