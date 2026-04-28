// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.HelperTests;

[TestFixture]
internal class IpHelperTests
{
    [Test]
    public void IsLocalPortAvailable_Port33002_IsAvailable()
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
        var result = IpHelper.IsRemotePortOpenAsync(ip, 33005).GetAwaiter().GetResult();

        // Assert

        Assert.That(result, Is.True);
    }

}