// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class ClientSocketProxyFactoryTests
{
    private readonly IPAddress _ipAddress = IPAddress.Parse("127.0.0.1");
    private readonly int _port = 9999;

    private readonly TcpIpListenerManager _tilm = TcpIpServerTestHelper.TcpIpListenerManager;

    [Test]
    public void CreateInstance_Client_Tcp_InstanceCreated()
    {
        // Arrange 
        var factory = new SocketProxyFactory(_tilm);

        var protocol = IpProtocolEnum.Tcp;

        // Act  
        var result = factory.CreateInstance(false,protocol, _ipAddress, _port, TestDataHelper.Logger);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetType().Name, Is.EqualTo(nameof(TcpIpClientSocketProxy)));
        }
    }

    [Test]
    public void CreateInstance_Client_Udp_InstanceCreated()
    {
        // Arrange 
        var factory = new SocketProxyFactory(_tilm);

        var protocol = IpProtocolEnum.Udp;

        // Act  
        var result = factory.CreateInstance(false,protocol, _ipAddress, _port, TestDataHelper.Logger);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetType().Name, Is.EqualTo(nameof(UdpClientSocketProxy)));
        }
    }

    [Test]
    public void CreateInstance_Server_Tcp_InstanceCreated()
    {
        // Arrange 
        var factory = new SocketProxyFactory(_tilm);

        var protocol = IpProtocolEnum.Tcp;

        // Act  
        var result = factory.CreateInstance(true, protocol, _ipAddress, _port, TestDataHelper.Logger);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(TcpIpServerSocketProxy)));
    }

    [Test]
    public void CreateInstance_Server_Udp_InstanceCreated()
    {
        // Arrange 
        var factory = new SocketProxyFactory(TcpIpServerTestHelper.TcpIpListenerManager);

        var protocol = IpProtocolEnum.Udp;

        // Act  
        var result = factory.CreateInstance(true, protocol, _ipAddress, _port, TestDataHelper.Logger);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetType().Name, Is.EqualTo(nameof(UdpServerSocketProxy)));
    }
}