// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Testing;

namespace Bodoconsult.NetworkCommunication.Tests.Testing;

[TestFixture]
internal class UdpTestMultiCastClientTests
{

    [Test]
    public void SendReceive_ValidSetup_MessageReceived()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("224.0.0.42");
        const int port = 65000;

        var server = new UdpTestMultiCastServer(ip, port);
        server.Start();

        var client = new UdpTestMultiCastClient(ip, port);
        client.Start();

        // Act  
        server.Send(data);

        Wait.Until(() => false, 500);

        // Assert

        Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));

        var msg = client.ReceivedMessages[0];

        Assert.That(msg.Length, Is.EqualTo(data.Length));
        Assert.That(msg.Span[0], Is.EqualTo(data[0]));
        Assert.That(msg.Span[1], Is.EqualTo(data[1]));
        Assert.That(msg.Span[2], Is.EqualTo(data[2]));

        server.Dispose();
        client.Dispose();
    }

}