// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Testing;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class TcpTestServerTests
{
    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();
        var serverCount = 0;

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var task = Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            var udpServer = new TcpTestServer(ip, port);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await udpServer.Receive(); // listen on port 11000
                    udpServer.Send(serverData); // reply back
                }

                serverCount = udpServer.ReceivedMessages.Count;

            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                udpServer.Dispose();
            }
        });

        await Task.Delay(100);

        // Act  
        var client = new TcpTestClient(ip, port);

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send(clientData);

            // then receive data
            await client.Receive();
        }

        client.Dispose();

        task.Wait();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(serverCount, Is.GreaterThan(3));
        }
    }

    [Test]
    public void SendReceive_SingleMessages_MessageReceived()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var server = new TcpTestServer(ip, port);
        server.Start();

        var client = new TcpTestClient(ip, port);
        client.Start();
        client.Send(data);

        Wait.Until(() => !server.ReceivedMessages.IsEmpty);

        // Act  
        server.Send(data);

        Wait.Until(() => false, 500);

        // Assert
        Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));


        client.Dispose();

        var success = client.ReceivedMessages.TryTake(out var msg);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(msg.Length, Is.EqualTo(data.Length));
            Assert.That(msg.Span[0], Is.EqualTo(data[0]));
            Assert.That(msg.Span[1], Is.EqualTo(data[1]));
            Assert.That(msg.Span[2], Is.EqualTo(data[2]));
        }
    }
}