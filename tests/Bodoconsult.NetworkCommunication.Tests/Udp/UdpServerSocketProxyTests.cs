// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class UdpServerSocketProxyTests
{
    /// <summary>
    /// Timeout in ms for waiting for messages to be delivered to next step
    /// </summary>
    private const int TimeOut = 2000;

    private readonly ConcurrentBag<ReadOnlyMemory<byte>> _clientReceivedMessages = [];

    [Test]
    public async Task SendReceive_PermanentModeTestClient_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerSocketProxy(TestDataHelper.Logger);
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();

            started.Set();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    //var data = new byte[10];
                    //var count = await udpServer.Receive(data);

                    //if (count > 0)
                    //{
                    //    _serverReceivedMessages.Add(data);
                    //}
                    //Debug.Print($"Server: received {data.Length} bytes");

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
        var client = new UdpClientSocketProxy(TestDataHelper.Logger);
        client.IpAddress = ip;
        client.Port = port;
        await client.Connect();

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            //// send data
            //client.Send(clientData);

            var data = new byte[10];

            // then receive data
            var result = await client.Receive(data);

            if (result > 0)
            {
                _clientReceivedMessages.Add(data);
            }
        }

        client.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task SendReceive_PermanentModeClientStartsEarlier_MessageReceived()
    {
        // Arrange 
        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        var serverData = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {
            var udpServer = new UdpServerSocketProxy(TestDataHelper.Logger);
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();

            started.WaitOne(TimeOut);

            await Task.Delay(100);

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

        // Act  
        var udpClient = new UdpClientSocketProxy(TestDataHelper.Logger);
        udpClient.IpAddress = ip;
        udpClient.Port = port;

        await udpClient.Connect();

        started.Set();

        while (!cts.IsCancellationRequested)
        {
            //// send data
            //var sent = await udpClient.Send(clientData);
            //Debug.Print($"Client: sent {sent} bytes");

            // then receive data
            var data = new byte[10];
            var count = await udpClient.Receive(data);

            if (count > 0)
            {
                _clientReceivedMessages.Add(data);
            }

            Debug.Print($"Client: received {data.Length} bytes");


        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }


    [Test]
    public async Task DuplexIoSendReceive_PermanentModeClientStartsEarlier_MessageReceived()
    {
        // Arrange 

        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { DeviceCommunicationBasics.Stx, 0x0, 0x48, 0x49, 0x50, 0x51, 0x52, DeviceCommunicationBasics.Etx };

        var msg = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                Data = clientData
            }
        };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var serverConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port
        };
        serverConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig);

        var clientConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port,
            RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate
        };

        clientConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(clientConfig);

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            started.WaitOne(3 * TimeOut);

            var udpServer = new UdpServerSocketProxy(TestDataHelper.Logger);
            serverConfig.SocketProxy = udpServer;
            udpServer.IpAddress = ip;
            udpServer.Port = port;

            var serverDuplexIo = new UdpDatagramSendOnlyIpDuplexIo(serverConfig, new SendPacketProcessFactory());
            await serverDuplexIo.StartCommunication();

            await udpServer.Connect();

            await Task.Delay(100);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    // send data
                    var result = await serverDuplexIo.SendMessageDirect(msg);
                    Debug.Print($"Server: sent {result.ProcessExecutionResult}");
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                await serverDuplexIo.DisposeAsync();
            }
        });

        // ReSharper disable once MethodSupportsCancellation

        // Act  
        var udpClient = new UdpClientSocketProxy(TestDataHelper.Logger);
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        clientConfig.SocketProxy = udpClient;

        await udpClient.Connect();


        var clientDuplexIo = new UdpDatagramReceiveOnlyIpDuplexIo(clientConfig, new SendPacketProcessFactory());

        await clientDuplexIo.StartCommunication();

        var count = 0;
        while (!cts.IsCancellationRequested)
        {
            await Task.Delay(10);

            if (count == 10)
            {
                started.Set();
            }
            count++;
        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task DuplexIoSendReceive_PermanentModeUdpClientSocketProxy_MessageReceived()
    {
        // Arrange 

        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x48, 0x49, 0x50, 0x51, 0x52 };


        var msg = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                Data = clientData
            }
        };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var serverConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port
        };
        serverConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig);

        var clientConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port,
            RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate
        };

        clientConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(clientConfig);

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            started.WaitOne(3 * TimeOut);

            var udpServer = new UdpServerSocketProxy(TestDataHelper.Logger);
            serverConfig.SocketProxy = udpServer;
            udpServer.IpAddress = ip;
            udpServer.Port = port;

            var serverDuplexIo = new UdpDatagramSendOnlyIpDuplexIo(serverConfig, new SendPacketProcessFactory());
            await serverDuplexIo.StartCommunication();

            await udpServer.Connect();

            await Task.Delay(100);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    // send data
                    var result = await serverDuplexIo.SendMessageDirect(msg);
                    Debug.Print($"Server: sent {result.ProcessExecutionResult}");
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            finally
            {
                await serverDuplexIo.DisposeAsync();
            }
        });

        // ReSharper disable once MethodSupportsCancellation

        // Act  
        var udpClient = new UdpClientSocketProxy(TestDataHelper.Logger);
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        clientConfig.SocketProxy = udpClient;

        await udpClient.Connect();


        var clientDuplexIo = new UdpDatagramReceiveOnlyIpDuplexIo(clientConfig, new SendPacketProcessFactory());

        await clientDuplexIo.StartCommunication();

        var count = 0;
        while (!cts.IsCancellationRequested)
        {
            await Task.Delay(10);

            if (count == 10)
            {
                started.Set();
            }
            count++;
        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_clientReceivedMessages.Count, Is.GreaterThan(3));
            //Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    private void RaiseCommLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        Debug.Print($"Server: received {message.RawMessageData.Length} bytes");
        _clientReceivedMessages.Add(message.RawMessageData);
    }
}
