// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using System.Net;
using Bodoconsult.NetworkCommunication.Testing;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

[Explicit]
[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpClientSocketProxyTests : BaseTestsTcpIpSocket
{
    private readonly TcpTestServer _server;

    public TcpIpClientSocketProxyTests()
    {
        _server = new TcpTestServer(IPAddress.Parse(IpAddress), Port);
        _server.Start();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _server.Dispose();
    }

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

        // Act  
        var socket = (TcpIpClientSocketProxy)Socket;

        // Assert
        Assert.That(socket.Socket, Is.Null);
    }
}