// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.NetworkCommunication.Helpers;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Tests.HelperTests;

[TestFixture]
internal class IpHelperTests
{
    [Test]
    public async Task IsPingableAsync_LocalHost_IsAvailable()
    {
        // Arrange 

        // Act  
        var result = await IpHelper.IsPingableAsync("127.0.0.1");

        // Assert

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsConnectableAsync_LocalHost_ReturnsFalse()
    {
        // Arrange 

        // Act  
        var result = await IpHelper.IsConnectableAsync("127.0.0.1", 33022);

        // Assert

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsConnectableAsync_LocalHost_ReturnsTrue()
    {
        // Arrange 
        using var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33022));

        // Act  
        var result = await IpHelper.IsConnectableAsync("127.0.0.1", 33022);

        // Assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void IsLocalPortAvailable_Port33002_ReturnsTrue()
    {
        // Arrange 

        // Act  
        var result = IpHelper.IsLocalPortAvailable(33005);

        // Assert

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsRemotePortAvailableAsync_Port33002_IsAvailable()
    {
        // Arrange 
        var ip = IpHelper.GetLocalIpAddress().ToString();

        // Act  
        Assert.DoesNotThrow(() =>
        {
            var result = IpHelper.IsRemotePortOpenAsync(ip, 33005).GetAwaiter().GetResult();
        });

        // Assert
        Assert.Pass();
    }

}