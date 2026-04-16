// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpClient.Bll.BusinessLogic;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Client;

[TestFixture]
internal class ClientUiManagerTests
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
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        // Act
        var m = new ClientUiManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.BackendTcpIp, Is.Null);
            Assert.That(m.BackendTcpIpConfig, Is.Null);
        }
    }

    [Test]
    public void LoadClient_ValidSetup_ClientLoaded()
    {
        // Arrange 
        var clientConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        var m = new ClientUiManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory)
        {
            BackendTcpIpConfig = clientConfig,
        };

        // Act
        m.LoadBackendTcpIp();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.BackendTcpIp, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(m.BackendTcpIp);
            Assert.That(m.BackendTcpIpConfig, Is.Not.Null);

            Assert.That(m.BackendTcpIp, Is.Not.Null);
            Assert.That(m.BackendTcpIp.IpDevice, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.BackendTcpIp.IpDevice);
            Assert.That(m.BackendTcpIp.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.BackendTcpIp.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(clientConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(clientConfig.Port));
            Assert.That(config.DataMessageProcessingPackage, Is.TypeOf<BtcpDataMessageProcessingPackage>());
        }
    }
}