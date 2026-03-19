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
    private readonly DoNothingOrderManagementClientNotificationManager _clientNotificationManager = new();
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();
    private readonly FakeSendPacketProcessFactory _sendPacketProcessFactory = new();

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        // Act  
        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Null);
        Assert.That(conf.Device, Is.Null);
    }

    [Test]
    public void CreateMessagingConfig_ValidSetup_MessagingConfigIsCreated()
    {
        // Arrange 
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);

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
        var duplexIoFactory = new IpDuplexIoFactory(_sendPacketProcessFactory);

        var conf = new UdpClientDeviceConfigurator(duplexIoFactory, _appEventSourceFactory, _clientNotificationManager);
        conf.CreateMessagingConfig("TestDevice", "127.0.0.1", 9000);

        // Act  
        conf.CreateDevice();

        // Assert
        Assert.That(conf.DataMessagingConfig, Is.Not.Null);
        Assert.That(conf.Device, Is.Not.Null);
    }
}