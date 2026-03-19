// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Factories;
using Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class BasicManagementDeviceFactoryTests
{
    [Test]
    public void CreateInstance_ValidSetup_ReturnsDevice()
    {
        // Arrange 
        var clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        var socketProxyFactory = new SocketProxyFactory(null);
        var sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        var appEventSourceFactory = new FakeAppEventSourceFactory();

        var communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory, appEventSourceFactory, clientNotificationManager);
        var outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory, outboundDataMessageFactory);

        var factory = new BasicManagementDeviceFactory(clientNotificationManager, commAdapterFactory);

        var dataMessagingConfig = TestDataHelper.GetDataMessagingConfig();
        dataMessagingConfig.StateMachineProcessingPackage = new StateMachineProcessingPackage();
        dataMessagingConfig.StateMachineProcessingPackage.StateMachineStateFactory = new StateMachineStateFactory();
        dataMessagingConfig.StateMachineProcessingPackage.StateCheckManager = new DoNothingStateCheckManager();

        // Act 
        var result = factory.CreateInstance(dataMessagingConfig);

        // Assert
        Assert.That(result.ClientNotificationManager, Is.EqualTo(clientNotificationManager));
        Assert.That(result.CommunicationAdapter, Is.Not.Null);
        Assert.That(result.DataMessagingConfig, Is.EqualTo(dataMessagingConfig));
    }
}