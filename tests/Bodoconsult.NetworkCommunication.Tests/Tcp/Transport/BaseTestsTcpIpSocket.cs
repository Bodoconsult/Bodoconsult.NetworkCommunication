// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Transport;

/// <summary>
/// Base class for TCP/IP socket implementations
/// </summary>
public abstract class BaseTestsTcpIpSocket
{
    protected ISocketProxy? Socket;

    protected IPEndPoint? CurrentIpEndPoint;

    /// <summary>
    /// IP address (use the one from test device development)
    /// </summary>
    protected const string IpAddress = "127.0.0.1";

    /// <summary>
    /// Default port for IP device
    /// </summary>
    protected int Port = TestDataHelper.GetRandomPort();

    [TearDown]
    public void Dispose()
    {
        Socket?.Dispose();
    }

    [Test]
    public void Connect_ValidEndpoint_Connected()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(Socket);

        // Act  
        Assert.DoesNotThrow(() =>
        {
            Socket.Connect();
        });

        // Assert
        Assert.That(Socket.Connected);
    }
}