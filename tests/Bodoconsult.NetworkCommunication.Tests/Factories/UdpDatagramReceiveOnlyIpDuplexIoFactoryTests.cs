// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class UdpDatagramReceiveOnlyIpDuplexIoFactoryTests
{
    [Test]
    public void TcpIpDuplexIoFactory_CreateInstance_Success()
    {
        // Arrange 
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        var factory = new UdpDatagramReceiveOnlyIpDuplexIoFactory(sendPacketProcessFactory);

        var socketProxy = new FakeTcpIpSocketProxy();

        var config = TestDataHelper.GetDataMessagingConfig();

        config.SocketProxy = socketProxy;

        // Act  
        var instance = factory.CreateInstance(config);

        // Assert
        Assert.That(instance, Is.Not.Null);
    }
}