// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp;

[Explicit]
[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class UdpServerWithHelloSocketProxyTests
{
    /// <summary>
    /// Timeout in ms for waiting for messages to be delivered to next step
    /// </summary>
    private const int TimeOut = 2000;

    private readonly ConcurrentBag<ReadOnlyMemory<byte>> _clientReceivedMessages = [];
    private readonly ConcurrentBag<ReadOnlyMemory<byte>> _serverReceivedMessages = [];

    private void ClientSocketReceivedDataDelegate(Memory<byte> data)
    {
        _clientReceivedMessages.Add(data);
    }

    private void ServerSocketReceivedDataDelegate(Memory<byte> data)
    {
        _serverReceivedMessages.Add(data);
    }

    [SetUp]
    public void Setup()
    {
        _clientReceivedMessages.Clear();
        _serverReceivedMessages.Clear();
    }

    [Test]
    public async Task SendReceive_PermanentModeTestClient_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerWithHelloSocketProxy(TestDataHelper.Logger);
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();
            udpServer.StartReceiverLoop(ServerSocketReceivedDataDelegate);

            started.Set();

            try
            {
                while (!cts.IsCancellationRequested)
                {
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
        started.WaitOne(TimeOut);

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
            Assert.That(_serverReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerWithHelloSocketProxy(TestDataHelper.Logger);
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();
            udpServer.StartReceiverLoop(ServerSocketReceivedDataDelegate);

            started.Set();

            try
            {
                while (!cts.IsCancellationRequested)
                {
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
        started.WaitOne(TimeOut);
        started.Reset();

        // Act  
        var udpClient = new UdpClientWithHelloSocketProxy(TestDataHelper.Logger);
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        await udpClient.Connect();
        udpClient.StartReceiverLoop(ClientSocketReceivedDataDelegate);

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await udpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");
        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_serverReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }
}