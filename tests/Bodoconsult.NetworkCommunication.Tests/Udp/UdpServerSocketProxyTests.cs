// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.Testing;
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

    ConcurrentBag<ReadOnlyMemory<byte>> serverReceivedMessages = [];

    [Test]
    public async Task SendReceive_PermanentModeTestClient_MessageReceived()
    {
        // Arrange 
        ConcurrentBag<ReadOnlyMemory<byte>> serverReceivedMessages = [];

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
            var udpServer = new UdpServerSocketProxy();
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();

            started.Set();

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
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    //    [Test]
    //    public async Task SendReceive_PermanentMode_MessageReceived()
    //    {
    //        // Arrange 
    //        ConcurrentBag<ReadOnlyMemory<byte>> serverReceivedMessages = [];
    //        ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

    //        var serverData = new byte[] { 0x0, 0x1, 0x2 };
    //        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

    //        var ip = IPAddress.Parse("127.0.0.1");
    //        var port = TestDataHelper.GetRandomPort();

    //        var cts = new CancellationTokenSource(5000);

    //        AutoResetEvent started = new(false);

    //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    //        // ReSharper disable once MethodSupportsCancellation
    //        Task.Run(async () =>
    //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    //        {
    //            var udpServer = new UdpServerSocketProxy();
    //            udpServer.IpAddress = ip;
    //            udpServer.Port = port;
    //            await udpServer.Connect();

    //            started.Set();

    //            try
    //            {
    //                while (!cts.IsCancellationRequested)
    //                {
    //                    var data = new byte[10];
    //                    var count = await udpServer.Receive(data);

    //                    if (count > 0)
    //                    {
    //                        serverReceivedMessages.Add(data);
    //                    }

    //                    Debug.Print($"Server: received {data.Length} bytes");

    //                    var sent = await udpServer.Send(serverData); // reply back
    //                    Debug.Print($"Server: sent {sent} bytes");
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Print(e.ToString());
    //            }
    //            finally
    //            {
    //                udpServer.Dispose();
    //            }
    //        });

    //        // ReSharper disable once MethodSupportsCancellation
    //        started.WaitOne(TimeOut);
    //        started.Reset();

    //        // Act  
    //        var udpClient = new UdpClientSocketProxy();
    //        udpClient.IpAddress = ip;
    //        udpClient.Port = port;

    //        await udpClient.Connect();

    //        //client.Start();

    //        while (!cts.IsCancellationRequested)
    //        {
    //            // send data
    //            var sent = await udpClient.Send(clientData);
    //            Debug.Print($"Client: sent {sent} bytes");

    //            // then receive data
    //            var data = new byte[10];
    //            var count = await udpClient.Receive(data);

    //            if (count > 0)
    //            {
    //                clientReceivedMessages.Add(data);
    //            }

    //            Debug.Print($"Client: received {data.Length} bytes");


    //        }

    //        udpClient.Dispose();

    //        // Assert
    //        using (Assert.EnterMultipleScope())
    //        {
    //            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
    //            Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
    //        }
    //    }

    [Test]
    public async Task SendReceive_PermanentModeClientStartsEarlier_MessageReceived()
    {
        // Arrange 
        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
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
            var udpServer = new UdpServerSocketProxy();
            udpServer.IpAddress = ip;
            udpServer.Port = port;
            await udpServer.Connect();

            started.WaitOne(TimeOut);

            await Task.Delay(100);

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

                    //var sent = await udpServer.Send(serverData); // reply back
                    //Debug.Print($"Server: sent {sent} bytes");


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

        started.Reset();

        // Act  
        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;

        await udpClient.Connect();

        started.Set();

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await udpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");

            //// then receive data
            //var data = new byte[10];
            //var count = await udpClient.Receive(data);

            //if (count > 0)
            //{
            //    clientReceivedMessages.Add(data);
            //}

            //Debug.Print($"Client: received {data.Length} bytes");


        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            //Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }


    [Test]
    public async Task DuplexIoSendReceive_PermanentModeClientStartsEarlier_MessageReceived()
    {
        // Arrange 

        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { DeviceCommunicationBasics.Stx, 0x0, 0x48, 0x49, 0x50, 0x51, 0x52, DeviceCommunicationBasics.Etx };

        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var serverConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port
        };
        serverConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig);
        serverConfig.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            var udpServer = new UdpServerSocketProxy();
            serverConfig.SocketProxy = udpServer;
            udpServer.IpAddress = ip;
            udpServer.Port = port;


            var serverDuplexIo = new UdpDatagramReceiveOnlyIpDuplexIo(serverConfig, new SendPacketProcessFactory());
            await serverDuplexIo.StartCommunication();

            await udpServer.Connect();

            started.WaitOne(TimeOut);

            await Task.Delay(100);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(10);


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
        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;

        await udpClient.Connect();

        started.Set();

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await udpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");

            //// then receive data
            //var data = new byte[10];
            //var count = await udpClient.Receive(data);

            //if (count > 0)
            //{
            //    clientReceivedMessages.Add(data);
            //}

            //Debug.Print($"Client: received {data.Length} bytes");


        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            //Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task DuplexIoSendReceive_PermanentModeClientStartsEarlierReadOnlyMemory_MessageReceived()
    {
        // Arrange 

        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new ReadOnlyMemory<byte>([DeviceCommunicationBasics.Stx, 0x0, 0x48, 0x49, 0x50, 0x51, 0x52, DeviceCommunicationBasics.Etx]);


        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var serverConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port
        };
        serverConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig);
        serverConfig.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            var udpServer = new UdpServerSocketProxy();
            serverConfig.SocketProxy = udpServer;
            udpServer.IpAddress = ip;
            udpServer.Port = port;


            var serverDuplexIo = new UdpDatagramReceiveOnlyIpDuplexIo(serverConfig, new SendPacketProcessFactory());
            await serverDuplexIo.StartCommunication();

            await udpServer.Connect();

            started.WaitOne(TimeOut);

            await Task.Delay(100);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(10);


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
        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;

        await udpClient.Connect();

        started.Set();

        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            var sent = await udpClient.Send(clientData);
            Debug.Print($"Client: sent {sent} bytes");

            //// then receive data
            //var data = new byte[10];
            //var count = await udpClient.Receive(data);

            //if (count > 0)
            //{
            //    clientReceivedMessages.Add(data);
            //}

            //Debug.Print($"Client: received {data.Length} bytes");


        }

        udpClient.Dispose();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            //Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    [Test]
    public async Task DuplexIoSendReceive_PermanentModeUdpClientSocketProxy_MessageReceived()
    {
        // Arrange 

        //ConcurrentBag<ReadOnlyMemory<byte>> clientReceivedMessages = [];

        //var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x48, 0x49, 0x50, 0x51, 0x52 };


        var ip = IPAddress.Parse("127.0.0.1");
        var port = TestDataHelper.GetRandomPort();

        var serverConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port
        };
        serverConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig);
        serverConfig.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var clientConfig = new DefaultDataMessagingConfig
        {
            IpAddress = "127.0.0.1",
            Port = port,
            DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(serverConfig)
        };

        //clientConfig.DataMessageProcessingPackage.DataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var cts = new CancellationTokenSource(5000);

        AutoResetEvent started = new(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // ReSharper disable once MethodSupportsCancellation
        Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            started.WaitOne(3 * TimeOut);

            var udpServer = new UdpServerSocketProxy();
            serverConfig.SocketProxy = udpServer;
            udpServer.IpAddress = ip;
            udpServer.Port = port;

            var serverDuplexIo = new UdpDatagramReceiveOnlyIpDuplexIo(serverConfig, new SendPacketProcessFactory());
            await serverDuplexIo.StartCommunication();

            await udpServer.Connect();

           await Task.Delay(100);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(10);


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
        var udpClient = new UdpClientSocketProxy();
        udpClient.IpAddress = ip;
        udpClient.Port = port;
        clientConfig.SocketProxy = udpClient;

        await udpClient.Connect();

        var msg = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                Data = clientData
            }
        };

        var clientDuplexIo = new UdpDatagramSendOnlyIpDuplexIo(clientConfig, new SendPacketProcessFactory());

        await clientDuplexIo.StartCommunication();

        var count = 0;
        while (!cts.IsCancellationRequested)
        {
            // send data

            var result = await clientDuplexIo.SendMessageDirect(msg);

            Debug.Print($"Client: sent {result.ProcessExecutionResult}");

            //// then receive data
            //var data = new byte[10];
            //var count = await udpClient.Receive(data);

            //if (count > 0)
            //{
            //    clientReceivedMessages.Add(data);
            //}

            //Debug.Print($"Client: received {data.Length} bytes");

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
            Assert.That(serverReceivedMessages.Count, Is.GreaterThan(3));
            //Assert.That(clientReceivedMessages.Count, Is.GreaterThan(3));
        }
    }

    private void RaiseCommLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        Debug.Print($"Server: received {message.RawMessageData.Length} bytes");
        serverReceivedMessages.Add(message.RawMessageData);
    }
}
