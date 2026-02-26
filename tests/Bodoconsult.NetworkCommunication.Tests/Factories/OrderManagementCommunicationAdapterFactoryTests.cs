// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.App;
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

        ISocketProxyFactory socketProxyFactory = new ClientSocketProxyFactory();
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        IDuplexIoFactory duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        ILogDataFactory logDataFactory = TestDataHelper.LogDataFactory;
        IAppLoggerProxyFactory appLoggerFactory = new AppLoggerProxyFactory();
        IAppEventSourceFactory appEventSourceFactory = new FakeAppEventSourceFactory();

        var commHandlerfactory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory,
            monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory);

        IOutboundDataMessageFactory outboundDataMessageFactory = new BtcpOutboundDataMessageFactory();

        var factory = new OrderManagementCommunicationAdapterFactory(commHandlerfactory, outboundDataMessageFactory);

        // Act  
        var instance = factory.CreateInstance(config);

        // Assert
        Assert.That(instance, Is.Not.Null);
    }
}