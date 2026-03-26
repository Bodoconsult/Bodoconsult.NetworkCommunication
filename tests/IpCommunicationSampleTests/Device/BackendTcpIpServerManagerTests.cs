// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Device.Bll.Communication;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Device;

[TestFixture]
internal class BackendTcpIpServerManagerTests
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
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);
 
        // Act
        var m = new BackendTcpIpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger);

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

        var m = new BackendTcpIpServerManager(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLogger);

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