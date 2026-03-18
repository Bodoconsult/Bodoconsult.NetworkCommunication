// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.App;
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


        ISocketProxyFactory socketProxyFactory = new ClientSocketProxyFactory();
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        IDuplexIoFactory duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        ILogDataFactory logDataFactory = TestDataHelper.LogDataFactory;
        IAppLoggerProxyFactory appLoggerFactory = new AppLoggerProxyFactory();
        IAppEventSourceFactory appEventSourceFactory = new FakeAppEventSourceFactory();

        ICentralClientNotificationManager clientNotificationManager = new FakeOrderManagementClientNotificationManager();

        var factory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory,
            monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory, clientNotificationManager);

        // Act  
        var instance = factory.CreateInstance(config);

        // Assert
        Assert.That(instance, Is.Not.Null);
    }

}