// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class ClientSocketProxyFactoryTests
{
    private readonly IPAddress _ipAddress = IPAddress.Parse("127.0.0.1");
    private readonly int _port = 9999;

    [Test]
    public void CreateInstance_Tcp_InstanceCreated()
    {
        // Arrange 
        var factory = new ClientSocketProxyFactory();

        var protocol = IpProtocolEnum.Tcp;

        // Act  
        var result = factory.CreateInstance(protocol, _ipAddress, _port);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(TcpIpClientSocketProxy)));
    }

    [Test]
    public void CreateInstance_Udp_InstanceCreated()
    {
        // Arrange 
        var factory = new ClientSocketProxyFactory();

        var protocol = IpProtocolEnum.Udp;

        // Act  
        var result = factory.CreateInstance(protocol, _ipAddress, _port);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(UdpClientSocketProxy)));
    }
}