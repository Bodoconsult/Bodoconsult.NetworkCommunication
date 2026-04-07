// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class SimpleDeviceFactoryTests
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
        var commAdapterFactory = new IpCommunicationAdapterFactory(communicationHandlerFactory);

        var factory = new SimpleDeviceFactory(clientNotificationManager, commAdapterFactory);

        var dataMessagingConfig = TestDataHelper.GetDataMessagingConfig();

        // Act 
        var result = factory.CreateInstance(dataMessagingConfig);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ClientNotificationManager, Is.EqualTo(clientNotificationManager));
            Assert.That(result.CommunicationAdapter, Is.Not.Null);
            Assert.That(result.DataMessagingConfig, Is.EqualTo(dataMessagingConfig));
        }
    }
}