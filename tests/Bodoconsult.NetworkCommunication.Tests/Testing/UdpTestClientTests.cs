// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Testing;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class UdpTestUniCastServerTests
{
    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var task = Task.Run(() =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpTestUniCastServer(ip, port);
            started.Set();
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    udpServer.Send(serverData); // reply back
                }
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

        started.WaitOne(1000);

        // Act  
        var client = new UdpTestUniCastClient(ip, port);
        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // then receive data
            await client.Receive();
        }

        var result = client.ReceivedMessages.Count;
        client.Dispose();

        task.Wait(5000);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task SendReceive_SingleMessages_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var task = Task.Run(() =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpTestUniCastServer(ip, port);

            started.Set();
            try
            {
                udpServer.Send(serverData); // reply back
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

        started.WaitOne(100);

        // Act  
        var client = new UdpTestUniCastClient(ip, port);

        while (!cts.IsCancellationRequested)
        {
            // then receive data
            var data = await client.Receive();
            if (data.Length > 0)
            {
                await cts.CancelAsync();
            }
        }

        var result = client.ReceivedMessages.Count;
        var success = client.ReceivedMessages.TryTake(out var msg);


        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Zero);
            Assert.That(success, Is.True);
            Assert.That(msg.Length, Is.EqualTo(serverData.Length));
            Assert.That(msg.Span[0], Is.EqualTo(serverData[0]));
            Assert.That(msg.Span[1], Is.EqualTo(serverData[1]));
            Assert.That(msg.Span[2], Is.EqualTo(serverData[2]));
        }

        client.Dispose();


    }
}