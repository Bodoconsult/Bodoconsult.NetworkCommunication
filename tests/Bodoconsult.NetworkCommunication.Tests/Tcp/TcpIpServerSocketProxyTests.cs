// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
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
    private readonly ConcurrentBag<ReadOnlySequence<byte>> _serverReceivedMessages = [];
    private readonly ConcurrentBag<ReadOnlySequence<byte>> _clientReceivedMessages = [];
    private readonly ConcurrentBag<Memory<byte>> _udpServerReceivedMessages = [];
    private readonly ConcurrentBag<Memory<byte>> _udpClientReceivedMessages = [];

    [SetUp]
    public void Setup()
    {
        _clientReceivedMessages.Clear();
        _serverReceivedMessages.Clear();
        _udpClientReceivedMessages.Clear();
        _udpServerReceivedMessages.Clear();
    }

    [Test]
    public async Task SendReceive_PermanentModeTestClient_MessageReceived()
    {
        // Arrange 

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
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager(), TestDataHelper.Logger);
            tcpServer.StartReceiverLoop(ServerSocketReceivedDataDelegate);
            tcpServer.IpAddress = ip;
            tcpServer.Port = port;
            tcpServer.Connect().GetAwaiter().GetResult();

            TestDataHelper.StartWaiting(cts, tcpServer.ReceiverPipeline, _serverReceivedMessages);


            try
            {
                while (!cts.IsCancellationRequested)
                {
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
        client.StartReceiverLoop();

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send(clientData);
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

        AutoResetEvent stopped = new(false);

        const int timeout = 5000;

        var cts = new CancellationTokenSource(timeout);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager(), TestDataHelper.Logger);
            tcpServer.LoggerId = "Server: ";
            tcpServer.IpAddress = ip;
            tcpServer.Port = port;
            await tcpServer.Connect();
            tcpServer.StartReceiverLoop(ServerSocketReceivedDataDelegate);

            TestDataHelper.StartWaiting(cts, tcpServer.ReceiverPipeline, _serverReceivedMessages);



            stopped.Set();

            //var connected = tcpServer.Connected;

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var sent = await tcpServer.Send(serverData); // send
                    Debug.Print($"Server: sent {sent} bytes");

                    await Task.Delay(20);
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
        stopped.WaitOne( 1000);

        // Act  
        var tcpClient = new TcpIpClientSocketProxy(TestDataHelper.Logger);
        tcpClient.LoggerId = "Client: ";
        tcpClient.IpAddress = ip;
        tcpClient.Port = port;
        await tcpClient.Connect();
        tcpClient.StartReceiverLoop(ClientSocketReceivedDataDelegate);
        TestDataHelper.StartWaiting(cts, tcpClient.ReceiverPipeline, _clientReceivedMessages);

        //client.Start();

        // TCP client handling
        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await tcpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");

            await Task.Delay(20);
        }

        tcpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_serverReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    private Task<bool> ClientSocketReceivedDataDelegate()
    {
        return Task.FromResult(true);
    }


    private Task<bool> ServerSocketReceivedDataDelegate()
    {
        //_serverReceivedMessages.Add(data);
        //Debug.Print($"Server: received {data.Length} bytes");
        return Task.FromResult(true);
    }



    [Test]
    public async Task SendReceive_PermanentModeUdpParallel_MessageReceived()
    {
        // Arrange 
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
            var tcpServer = new TcpIpServerSocketProxy(new TcpIpListenerManager(), TestDataHelper.Logger);
            
            tcpServer.IpAddress = ip;
            tcpServer.Port = port2;
            tcpServer.Connect().GetAwaiter().GetResult();
            tcpServer.StartReceiverLoop(ServerSocketReceivedDataDelegate);
            TestDataHelper.StartWaiting(cts, tcpServer.ReceiverPipeline, _serverReceivedMessages);

            try
            {
                while (!cts.IsCancellationRequested)
                {
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
            var udpServer = new UdpServerSocketProxy(TestDataHelper.Logger);
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();
            udpServer.StartReceiverLoop(UdpServerSocketReceivedDataDelegate);

            try
            {
                while (!cts.IsCancellationRequested)
                {
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
        var tcpClient = new TcpIpClientSocketProxy(TestDataHelper.Logger);
        tcpClient.IpAddress = ip;
        tcpClient.Port = port2;
        await tcpClient.Connect();
        tcpClient.StartReceiverLoop(ClientSocketReceivedDataDelegate);
        TestDataHelper.StartWaiting(cts, tcpClient.ReceiverPipeline, _clientReceivedMessages);

        var udpClient = new UdpClientSocketProxy(TestDataHelper.Logger);
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        await udpClient.Connect();
        udpClient.StartReceiverLoop(UdpClientSocketReceivedDataDelegate);

        // TCP client handling
        var tcpClientTask = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                // send data
                var sent = await tcpClient.Send(clientData);
                Debug.Print($"TcpClient: sent {sent} bytes");
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
            }

            udpClient.Dispose();
        });

        Wait.Until(() => tcpClientTask.IsCompleted, 2 * timeout);
        Wait.Until(() => udpClientTask.IsCompleted, 2 * timeout);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_serverReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(_udpServerReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(_udpClientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    private void UdpClientSocketReceivedDataDelegate(Memory<byte> data)
    {
        _udpClientReceivedMessages.Add(data);
        Debug.Print($"UdpClient: received {data.Length} bytes");
    }

    private void UdpServerSocketReceivedDataDelegate(Memory<byte> data)
    {
        _udpServerReceivedMessages.Add(data);
        Debug.Print($"UdpServer: received {data.Length} bytes");
    }
}