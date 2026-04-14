// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic;
using IpCommunicationSampleTests.App;


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
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        // Act
        var m = new BackendManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory);

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

    [Test]
    public void LoadClient_ValidSetup_ClientLoaded()
    {
        // Arrange 
        var clientConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33000 };
        var deviceTcpIpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33001 };
        var deviceUdpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        var m = new BackendManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory)
            {
                ClientTcpIpConfig = clientConfig,
                IpDeviceTcpIpConfig = deviceTcpIpConfig,
                IpDeviceUdpConfig = deviceUdpConfig
            };

        // Act
        m.LoadClient();

        // Assert
        using (Assert.EnterMultipleScope())
        {

            Assert.That(m.IpDeviceTcpIp, Is.Null);
            Assert.That(m.IpDeviceUdp, Is.Null);
            Assert.That(m.Client, Is.Not.Null);
            Assert.That(m.IpDeviceTcpIpConfig, Is.Not.Null);
            Assert.That(m.IpDeviceUdpConfig, Is.Not.Null);
            Assert.That(m.ClientTcpIpConfig, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.Client);
            ArgumentNullException.ThrowIfNull(m.Client.IpDevice);
            Assert.That(m.Client.IpDevice, Is.Not.Null);

            Assert.That(m.Client.IpDevice, Is.Not.Null);
            Assert.That(m.Client.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.Client.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(clientConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(clientConfig.Port));
        }
    }


    [Test]
    public void LoadIpDeviceTcpIp_ValidSetup_IpDeviceTcpIpLoaded()
    {
        // Arrange 
        var clientConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33000 };
        var deviceTcpIpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33001 };
        var deviceUdpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        var m = new BackendManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory)
        {
            ClientTcpIpConfig = clientConfig,
            IpDeviceTcpIpConfig = deviceTcpIpConfig,
            IpDeviceUdpConfig = deviceUdpConfig
        };

        // Act
        m.LoadIpDeviceTcpIp();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.IpDeviceTcpIp, Is.Not.Null);
            Assert.That(m.IpDeviceUdp, Is.Null);
            Assert.That(m.Client, Is.Null);
            Assert.That(m.IpDeviceTcpIpConfig, Is.Not.Null);
            Assert.That(m.IpDeviceUdpConfig, Is.Not.Null);
            Assert.That(m.ClientTcpIpConfig, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.IpDeviceTcpIp);
            ArgumentNullException.ThrowIfNull(m.IpDeviceTcpIp.IpDevice);
            Assert.That(m.IpDeviceTcpIp.IpDevice, Is.Not.Null);

            Assert.That(m.IpDeviceTcpIp.IpDevice, Is.Not.Null);
            Assert.That(m.IpDeviceTcpIp.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.IpDeviceTcpIp.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(deviceTcpIpConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(deviceTcpIpConfig.Port));
        }
    }

    [Test]
    public void LoadIpDeviceUdp_ValidSetup_IpDeviceUdpLoaded()
    {
        // Arrange 
        var clientConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33000 };
        var deviceTcpIpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33001 };
        var deviceUdpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var orderPipelineFactory = new OrderPipelineFactory(_dateService, _appLogger);
        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var socketFactory = new SocketProxyFactory(_tcpIpListenerManager);

        var m = new BackendManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _dateService, _syncOrderManager, _appBenchProxy, _orderReceiverFactory,
            _requestProcessorFactoryFactory, _requestStepProcessorFactoryFactory, orderPipelineFactory, _orderIdGenerator, businessTransactionManager, socketFactory)
        {
            ClientTcpIpConfig = clientConfig,
            IpDeviceTcpIpConfig = deviceTcpIpConfig,
            IpDeviceUdpConfig = deviceUdpConfig
        };

        // Act
        m.LoadIpDeviceUdp();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.IpDeviceTcpIp, Is.Null);
            Assert.That(m.IpDeviceUdp, Is.Not.Null);
            Assert.That(m.Client, Is.Null);
            Assert.That(m.IpDeviceTcpIpConfig, Is.Not.Null);
            Assert.That(m.IpDeviceUdpConfig, Is.Not.Null);
            Assert.That(m.ClientTcpIpConfig, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.IpDeviceUdp);
            ArgumentNullException.ThrowIfNull(m.IpDeviceUdp.IpDevice);
            Assert.That(m.IpDeviceUdp.IpDevice, Is.Not.Null);
            Assert.That(m.IpDeviceUdp.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.IpDeviceUdp.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(deviceUdpConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(deviceUdpConfig.Port));
        }
    }
}