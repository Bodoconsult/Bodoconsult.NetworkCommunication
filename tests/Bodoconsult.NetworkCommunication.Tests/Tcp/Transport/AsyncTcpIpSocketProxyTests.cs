// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

[Explicit]
[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class AsyncTcpIpSocketProxyTests : BaseTestsTcpIpSocket
{
    /// <summary>
    /// Setup for each test
    /// </summary>
    [SetUp]
    public void TestSetup()
    {
        Socket = new AsyncTcpIpSocketProxy();

        CurrentIpEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
    }

    [Test]
    public void Ctor_ValidEndpoint_SocketLoaded()
    {
        // Arrange 
        var socket = (AsyncTcpIpSocketProxy)Socket;

        // Act  
            

        // Assert
        Assert.That(socket.Socket, Is.Not.Null);

    }



}