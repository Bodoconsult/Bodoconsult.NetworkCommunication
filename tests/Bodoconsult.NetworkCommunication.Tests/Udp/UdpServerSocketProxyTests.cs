// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp;

[TestFixture]
internal class UdpServerSocketProxyTests
{
    [Test]
    public async Task SendReceive_PermanentModeTestClient_MessageReceived()
    {
        // Arrange 
        ConcurrentBag<ReadOnlyMemory<byte>> serverReceivedMessages = [];

    var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerSocketProxy();
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            udpServer.Connect().GetAwaiter().GetResult();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var data = new byte[10];
                    var count = await udpServer.Receive(data);

                    if (count > 0)
                    {
                        serverReceivedMessages.Add(data);
                    }
                    Debug.Print($"Server: received {data.Length} bytes");

                    var sent = await udpServer.Send(serverData); // reply back
                    Debug.Print($"Server: sent {sent} bytes");
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

        // ReSharper disable once MethodSupportsCancellation
        await Task.Delay(100);

        // Act  
        var client = new UdpTestUniCastClient(ip, port);
        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send(clientData);

            // then receive data
            await client.Receive();
        }

        client.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        ConcurrentBag<ReadOnlyMemory<byte>> serverReceivedMessages = [];
        ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerSocketProxy();
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            udpServer.Connect().GetAwaiter().GetResult();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var data = new byte[10];
                    var count = await udpServer.Receive(data);

                    if (count > 0)
                    {
                        serverReceivedMessages.Add(data);
                    }

                    Debug.Print($"Server: received {data.Length} bytes");

                    var sent = await udpServer.Send(serverData); // reply back
                    Debug.Print($"Server: sent {sent} bytes");
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

        // ReSharper disable once MethodSupportsCancellation
        await Task.Delay(100);

        // Act  
        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        udpClient.Connect().GetAwaiter().GetResult();

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await udpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");

            // then receive data
            var data = new byte[10];
            var count = await udpClient.Receive(data);

            if (count > 0)
            {
                clientReceivedMessages.Add(data);
            }

            Debug.Print($"Client: received {data.Length} bytes");
        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }
}