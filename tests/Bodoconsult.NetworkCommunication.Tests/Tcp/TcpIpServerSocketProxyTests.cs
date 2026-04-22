// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class TcpIpServerSocketProxyTests
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
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager());
            tcpServer.IpAddress = ip;
            tcpServer.Port = port;
            tcpServer.Connect().GetAwaiter().GetResult();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var data = new byte[10];
                    var count = await tcpServer.Receive(data);

                    if (count > 0)
                    {
                        serverReceivedMessages.Add(data);
                    }
                    Debug.Print($"Server: received {data.Length} bytes");

                    var sent = await tcpServer.Send(serverData); // reply back
                    Debug.Print($"Server: sent {sent} bytes");
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                tcpServer.Dispose();
            }
        });

        // ReSharper disable once MethodSupportsCancellation
        await Task.Delay(100);

        // Act  
        var client = new TcpTestClient(ip, port);
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

        const int timeout = 5000;

        var cts = new CancellationTokenSource(timeout);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager());
            tcpServer.IpAddress = ip;
            tcpServer.Port = port;
            tcpServer.Connect().GetAwaiter().GetResult();

            var connected = tcpServer.Connected;

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var data = new byte[10];
                    var count = await tcpServer.Receive(data);

                    if (count > 0)
                    {
                        serverReceivedMessages.Add(data);
                    }

                    Debug.Print($"Server: received {data.Length} bytes");

                    var sent = await tcpServer.Send(serverData); // reply back
                    Debug.Print($"Server: sent {sent} bytes");
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                tcpServer.Dispose();
            }
        });

        // ReSharper disable once MethodSupportsCancellation
        await Task.Delay(100);

        // Act  
        var tcpClient = new TcpIpClientSocketProxy();
        tcpClient.IpAddress = ip;
        tcpClient.Port = port;
        tcpClient.Connect().GetAwaiter().GetResult();

        //client.Start();

        // TCP client handling
            while (!cts.IsCancellationRequested)
            {
                // send data
                var sent = await tcpClient.Send(clientData);
                Debug.Print($"Client: sent {sent} bytes");

                // then receive data
                var data = new byte[10];
                var count = await tcpClient.Receive(data);

                if (count > 0)
                {
                    clientReceivedMessages.Add(data);
                }

                Debug.Print($"Client: received {data.Length} bytes");
            }

            tcpClient.Dispose();
       
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task SendReceive_PermanentModeUdpParallel_MessageReceived()
    {
        // Arrange 
        ConcurrentBag<ReadOnlyMemory<byte>> tcpServerReceivedMessages = [];
        ConcurrentBag<ReadOnlyMemory<byte>> tcpClientReceivedMessages = [];
        ConcurrentBag<ReadOnlyMemory<byte>> udpServerReceivedMessages = [];
        ConcurrentBag<ReadOnlyMemory<byte>> udpClientReceivedMessages = [];

        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = 33001;
        var port2 = 33002;

        const int timeout = 5000;

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager());
            tcpServer.IpAddress = ip;
            tcpServer.Port = port2;
            tcpServer.Connect().GetAwaiter().GetResult();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var data = new byte[10];
                    var count = await tcpServer.Receive(data);

                    if (count > 0)
                    {
                        tcpServerReceivedMessages.Add(data);
                    }

                    Debug.Print($"TcpServer: received {data.Length} bytes");

                    var sent = await tcpServer.Send(serverData); // reply back
                    Debug.Print($"TcpServer: sent {sent} bytes");
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                tcpServer.Dispose();
            }
        });

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
                        udpServerReceivedMessages.Add(data);
                    }

                    Debug.Print($"UdpServer: received {data.Length} bytes");

                    var sent = await udpServer.Send(serverData); // reply back
                    Debug.Print($"UdpServer: sent {sent} bytes");
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
        var tcpClient = new TcpIpClientSocketProxy();
        tcpClient.IpAddress = ip;
        tcpClient.Port = port2;
        tcpClient.Connect().GetAwaiter().GetResult();

        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        udpClient.Connect().GetAwaiter().GetResult();

        // TCP client handling
        var tcpClientTask = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                // send data
                var sent = await tcpClient.Send(clientData);
                Debug.Print($"TcpClient: sent {sent} bytes");

                // then receive data
                var data = new byte[10];
                var count = await tcpClient.Receive(data);

                if (count > 0)
                {
                    tcpClientReceivedMessages.Add(data);
                }

                Debug.Print($"TcpClient: received {data.Length} bytes");
            }

            tcpClient.Dispose();
        });

        var udpClientTask = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                // send data
                var sent = await udpClient.Send(clientData);
                Debug.Print($"UdpClient: sent {sent} bytes");

                // then receive data
                var data = new byte[10];
                var count = await udpClient.Receive(data);

                if (count > 0)
                {
                    udpClientReceivedMessages.Add(data);
                }

                Debug.Print($"UdpClient: received {data.Length} bytes");
            }

            udpClient.Dispose();
        });

        Wait.Until(() => tcpClientTask.IsCompleted, 2 * timeout);
        Wait.Until(() => udpClientTask.IsCompleted, 2 * timeout);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tcpServerReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(tcpClientReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(udpServerReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(udpClientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }
}