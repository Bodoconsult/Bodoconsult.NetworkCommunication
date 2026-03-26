// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Device.Bll;
using IpCommunicationSample.Device.Bll.Interfaces;
using IpCommunicationSampleTests.App;


namespace IpCommunicationSampleTests.Device;

[TestFixture]
internal class IpDeviceManagerTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly AppBenchProxy _appBenchProxy = TestDataHelper.GetFakeAppBenchProxy();
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
        var btm = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);

        // Act
        var m = new IpDeviceManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _tcpIpListenerManager, btm);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.BackendTcpIp, Is.Null);
            Assert.That(m.BackendUdp, Is.Null);
            Assert.That(m.BackendTcpIpConfig, Is.Null);
            Assert.That(m.BackendUdpConfig, Is.Null);
        }
    }

    [Test]
    public void LoadBackendTcpIp_ValidSetup_BackendTcpIpLoaded()
    {
        // Arrange 
        var deviceTcpIpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33001 };
        var deviceUdpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var btm = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);

        var m = new IpDeviceManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _tcpIpListenerManager, btm)
        {
            BackendTcpIpConfig = deviceTcpIpConfig,
            BackendUdpConfig = deviceUdpConfig
        };

        // Act
        m.LoadBackendTcpIp();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.BackendTcpIp, Is.Not.Null);
            Assert.That(m.BackendUdp, Is.Null);
            Assert.That(m.BackendTcpIpConfig, Is.Not.Null);
            Assert.That(m.BackendUdpConfig, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.BackendTcpIp);
            ArgumentNullException.ThrowIfNull(m.BackendTcpIp.IpDevice);
            Assert.That(m.BackendTcpIp.IpDevice, Is.Not.Null);

            Assert.That(m.BackendTcpIp.IpDevice, Is.Not.Null);
            Assert.That(m.BackendTcpIp.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.BackendTcpIp.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(deviceTcpIpConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(deviceTcpIpConfig.Port));
        }
    }

    [Test]
    public void LoadBackendUdp_ValidSetup_BackendUdpLoaded()
    {
        // Arrange 
        var deviceTcpIpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33001 };
        var deviceUdpConfig = new IpConfig { IpAddress = "127.0.0.1", Port = 33002 };

        var btm = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);

        var m = new IpDeviceManager(_monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _appLogger, _sendPacketProcessFactory, _tcpIpListenerManager, btm)
        {
            BackendTcpIpConfig = deviceTcpIpConfig,
            BackendUdpConfig = deviceUdpConfig
        };

        // Act
        m.LoadBackendUdp();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(m.BackendTcpIp, Is.Null);
            Assert.That(m.BackendUdp, Is.Not.Null);
            Assert.That(m.BackendTcpIpConfig, Is.Not.Null);
            Assert.That(m.BackendUdpConfig, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(m.BackendUdp);
            ArgumentNullException.ThrowIfNull(m.BackendUdp.IpDevice);
            Assert.That(m.BackendUdp.IpDevice, Is.Not.Null);

            Assert.That(m.BackendUdp.IpDevice, Is.Not.Null);
            Assert.That(m.BackendUdp.IpDevice.DataMessagingConfig, Is.Not.Null);

            var config = (IIpDataMessagingConfig)m.BackendUdp.IpDevice.DataMessagingConfig;

            Assert.That(config.IpAddress, Is.EqualTo(deviceUdpConfig.IpAddress));
            Assert.That(config.Port, Is.EqualTo(deviceUdpConfig.Port));
        }
    }
}