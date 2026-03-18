// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.Communication.IpDeviceUdp;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Backend.Configurators;

[TestFixture]
internal class IpDeviceUdpConfiguratorTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var clientNotificationManager = new FakeOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        var logDataFactory = TestDataHelper.LogDataFactory;
        var appLoggerFactory = new AppLoggerProxyFactory();
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        // Act  
        var conf = new IpDeviceUdpConfigurator(duplexIoFactory, monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory, clientNotificationManager);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Null);
        Assert.That(conf.Device, Is.Null);
    }

    [Test]
    public void CreateMessagingConfig_ValidSetup_MessagingConfigIsCreated()
    {
        // Arrange 
        var clientNotificationManager = new FakeOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        var logDataFactory = TestDataHelper.LogDataFactory;
        var appLoggerFactory = new AppLoggerProxyFactory();
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        var conf = new IpDeviceUdpConfigurator(duplexIoFactory, monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory, clientNotificationManager);

        // Act  
        conf.CreateMessagingConfig("127.0.0.1", 9000);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Null);
    }

    [Test]
    public void CreateDevice_ValidSetup_DeviceCreated()
    {
        // Arrange 
        var clientNotificationManager = new FakeOrderManagementClientNotificationManager();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        var logDataFactory = TestDataHelper.LogDataFactory;
        var appLoggerFactory = new AppLoggerProxyFactory();
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        var conf = new IpDeviceUdpConfigurator(duplexIoFactory, monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory, clientNotificationManager);
        conf.CreateMessagingConfig("127.0.0.1", 9000);

        // Act  
        conf.CreateDevice();

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Not.Null);
    }
}