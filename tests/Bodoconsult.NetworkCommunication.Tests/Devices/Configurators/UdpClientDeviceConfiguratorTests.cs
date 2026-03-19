// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Devices.Configurators;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Devices.Configurators;

[TestFixture]
internal class UdpClientDeviceConfiguratorTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        // Act  
        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, appEventSourceFactory, clientNotificationManager);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Null);
        Assert.That(conf.Device, Is.Null);
    }

    [Test]
    public void CreateMessagingConfig_ValidSetup_MessagingConfigIsCreated()
    {
        // Arrange 
        var clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, appEventSourceFactory, clientNotificationManager);

        const string ip = "127.0.0.1";
        const int port = 9000;

        // Act  
        conf.CreateMessagingConfig("TestDevice", ip, port);

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
        var clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        var logDataFactory = TestDataHelper.LogDataFactory;
        var appLoggerFactory = new AppLoggerProxyFactory();
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, appEventSourceFactory, clientNotificationManager);
        conf.CreateMessagingConfig("TestDevice", "127.0.0.1", 9000);

        // Act  
        conf.CreateDevice();

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Not.Null);
    }
}