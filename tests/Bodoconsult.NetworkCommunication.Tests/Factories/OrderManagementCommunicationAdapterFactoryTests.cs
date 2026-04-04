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
internal class OrderManagementCommunicationAdapterFactoryTests
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

        var commHandlerfactory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory,
             appEventSourceFactory, clientNotificationManager);

        IOutboundDataMessageFactory outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();

        var factory = new IpCommunicationAdapterFactory(commHandlerfactory, outboundDataMessageFactory);

        // Act  
        var instance = factory.CreateInstance(config);

        // Assert
        Assert.That(instance, Is.Not.Null);
    }
}