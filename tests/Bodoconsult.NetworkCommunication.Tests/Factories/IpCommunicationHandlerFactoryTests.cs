// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class IpCommunicationHandlerFactoryTests
{
    [Test]
    public void CreateInstance_ValdiSetup_InstanceCreated()
    {
        // Arrange 
        var config = TestDataHelper.GetDataMessagingConfig();


        ISocketProxyFactory socketProxyFactory = new SocketProxyFactory(null);
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        IDuplexIoFactory duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        IAppEventSourceFactory appEventSourceFactory = new FakeAppEventSourceFactory();

        ICentralClientNotificationManager clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        var factory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory,
             appEventSourceFactory, clientNotificationManager);

        // Act  
        var instance = factory.CreateInstance(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(instance, Is.Not.Null);
            Assert.That(config.SocketProxy, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(config.SocketProxy);
            Assert.That(config.SocketProxy.Logger, Is.Not.Null);
        }
    }

}