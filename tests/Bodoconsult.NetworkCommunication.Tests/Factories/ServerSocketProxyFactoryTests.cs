// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class ServerSocketProxyFactoryTests
{
    [Test]
    public void CreateInstance_Tcp_InstanceCreated()
    {
        // Arrange 
        var factory = new ServerSocketProxyFactory(TcpIpServerTestHelper.TcpIpListenerManager);

        var protocol = IpProtocolEnum.Tcp;

        // Act  
        var result = factory.CreateInstance(protocol);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(TcpIpServerSocketProxy)));
    }

    [Test]
    public void CreateInstance_Udp_InstanceCreated()
    {
        // Arrange 
        var factory = new ServerSocketProxyFactory(TcpIpServerTestHelper.TcpIpListenerManager);

        var protocol = IpProtocolEnum.Udp;

        // Act  
        var result = factory.CreateInstance(protocol);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(UdpServerSocketProxy)));
    }
}