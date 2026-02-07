// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

[Explicit]
[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpClientSocketProxyTests : BaseTestsTcpIpSocket
{
    /// <summary>
    /// Setup for each test
    /// </summary>
    [SetUp]
    public void TestSetup()
    {
        Socket = new TcpIpClientSocketProxy();
        Socket.IpAddress = IPAddress.Parse(IpAddress);
        Socket.Port = Port;

        CurrentIpEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
    }

    [Test]
    public void Ctor_ValidEndpoint_SocketLoaded()
    {
        // Arrange 
        var socket = (TcpIpClientSocketProxy)Socket;

        // Act  
            

        // Assert
        Assert.That(socket.Socket, Is.Not.Null);

    }



}