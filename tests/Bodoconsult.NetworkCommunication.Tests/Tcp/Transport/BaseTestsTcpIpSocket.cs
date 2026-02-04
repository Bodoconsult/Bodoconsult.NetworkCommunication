// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Net;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

/// <summary>
/// Base class for TCP/IP socket implementations
/// </summary>
public abstract class BaseTestsTcpIpSocket
{
    protected ISocketProxy Socket;

    protected IPEndPoint CurrentIpEndPoint;

    /// <summary>
    /// IP address (use the one from test tower development)
    /// </summary>
    protected const string IpAddress = "127.0.0.1";

    /// <summary>
    /// Default port for SMD tower (9001 simulator)
    /// </summary>
    protected const int Port = 9000;

    [TearDown]
    public void Dispose()
    {
        Socket?.Dispose();
    }

    [Test]
    public void Connect_ValidEndpoint_Connected()
    {
        // Arrange 

        // Act  
        Assert.DoesNotThrow(() =>
        {
            Socket.Connect();
        });

        // Assert
        Assert.That(Socket.Connected);
    }
}