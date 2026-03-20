// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Devices.Configurators;

[TestFixture]
internal class TcpIpServerDeviceConfiguratorTests
{
    private readonly IMonitorLoggerFactoryFactory _monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
    private readonly IAppLoggerProxy _appLoggerProxy = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly ISendPacketProcessFactory _sendPacketProcessFactory = new FakeSendPacketProcessFactory();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();
    private readonly TcpIpListenerManager _tcpIpListenerManager = new();
    private readonly LogDataFactory _logDataFactory = TestDataHelper.LogDataFactory;
    private readonly AppLoggerProxyFactory _appLoggerFactory = new();
    private readonly DoNothingOrderManagementClientNotificationManager _clientNotificationManager = new();
    private readonly IDataMessageProcessingPackageFactory _messageProcessingPackageFactory = new TncpDataMessageProcessingPackageFactory();

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appLoggerProxy.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
       var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);


        // Act  
        var conf = new TcpIpServerDeviceConfigurator(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory, 
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLoggerProxy);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Null);
        Assert.That(conf.Device, Is.Null);
    }

    [Test]
    public void CreateMessagingConfig_ValidSetup_MessagingConfigIsCreated()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var conf = new TcpIpServerDeviceConfigurator(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLoggerProxy);

        const string ip = "127.0.0.1";
        const int port = 9000;

        // Act  
        conf.CreateMessagingConfig("TestDevice",ip, port, _messageProcessingPackageFactory);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Null);
        Assert.That(conf.DataMessagingConfig.IpAddress, Is.EqualTo(ip));
        Assert.That(conf.DataMessagingConfig.Port, Is.EqualTo(port));
        Assert.That(conf.DataMessagingConfig.DataMessageProcessingPackage, Is.Not.Null);
        Assert.That(conf.DataMessagingConfig.StateMachineProcessingPackage, Is.Null);
    }

    [Test]
    public void CreateDevice_ValidSetup_DeviceCreated()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var conf = new TcpIpServerDeviceConfigurator(duplexIoFactory, _monitorLoggerFactoryFactory, _logDataFactory, _appLoggerFactory,
            _appEventSourceFactory, _clientNotificationManager, _tcpIpListenerManager, _appLoggerProxy);
        conf.CreateMessagingConfig("TestDevice","127.0.0.1", 9000, _messageProcessingPackageFactory);

        // Act  
        conf.CreateDevice();

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Not.Null);
    }
}