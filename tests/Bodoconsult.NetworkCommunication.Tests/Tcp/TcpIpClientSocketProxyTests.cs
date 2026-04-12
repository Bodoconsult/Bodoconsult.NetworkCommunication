// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

[TestFixture]
internal class TcpIpClientSocketProxyTests
{
    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        try
        {
            // Arrange 
            var serverData = new byte[] { 0x0, 0x1, 0x2 };
            var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

            var ip = IPAddress.Parse("127.0.0.1");
            var port = TestDataHelper.GetRandomPort();
            var serverCount = 0;

            var cts = new CancellationTokenSource(5000);

            var tcpIpServer = new TcpTestServer(ip, port);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                var client = new TcpIpClientSocketProxy();
                client.IpAddress = ip;
                client.Port = port;
                client.Connect().GetAwaiter().GetResult();

                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        var sent = await client.Send(serverData); // reply back
                        Debug.Print($"Client: sent {sent} bytes");

                        var data = new byte[10];
                        var received = await client.Receive(data); ; // listen on port 11000

                        Debug.Print($"Client: received {received} bytes");
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString());
                }
                finally
                {
                    client.Dispose();
                }
            });

            await Task.Delay(100);

            // Act
            while (!cts.IsCancellationRequested)
            {
                // then receive data
                await tcpIpServer.Receive();

                // send data
                tcpIpServer.Send(clientData);
            }

            serverCount = tcpIpServer.ReceivedMessages.Count;
            tcpIpServer.Dispose();

            // Assert
            Assert.That(serverCount, Is.GreaterThan(3));
        }
        catch (Exception e)
        {
            Debug.Print(e.ToString());
            Assert.Fail(e.ToString());
        }

    }

    //[Test]
    //public void SendReceive_SingleMessages_MessageReceived()
    //{
    //    // Arrange 
    //    var data = new byte[] { 0x0, 0x1, 0x2 };

    //    var ip = IPAddress.Parse("127.0.0.1");
    //    var port = TestDataHelper.GetRandomPort();

    //    var server = new TcpIpServerSocketProxy();
    //    server.IpAddress = ip;
    //    server.Port = port;
    //    server.Connect().GetAwaiter().GetResult();

    //    var client = new TcpIpTestUniCastClient(ip, port);
    //    client.Start();
    //    client.Send(data);

    //    // Act  
    //    server.Send(data);

    //    Wait.Until(() => false, 500);

    //    // Assert
    //    Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));

    //    server.Dispose();
    //    client.Dispose();

    //    var success = client.ReceivedMessages.TryTake(out var msg);

    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(success, Is.True);
    //        Assert.That(msg.Length, Is.EqualTo(data.Length));
    //        Assert.That(msg.Span[0], Is.EqualTo(data[0]));
    //        Assert.That(msg.Span[1], Is.EqualTo(data[1]));
    //        Assert.That(msg.Span[2], Is.EqualTo(data[2]));
    //    }
    //}
}