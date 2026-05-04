// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

[Explicit]
[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class UdpSocketTests
{
    [Test]
    public void Test1_UdpClientWithHello_UdpMessagesSentToClient()
    {
        // Arrange 
        // Source - https://stackoverflow.com/q/20038943
        // Posted by Tono Nam, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-04-09, License - CC BY-SA 3.0

        var cts = new CancellationTokenSource(2000);

        const int port = 11001;

        Task.Run(() =>
        {

            var udpServer = new UdpClient(port);
            var remoteEp = new IPEndPoint(IPAddress.Any, port);

            while (!cts.IsCancellationRequested)
            {
                var data = udpServer.Receive(ref remoteEp); // listen on port 11000
                Debug.Print($"Server: receive data from {remoteEp}: {data.Length} bytes");
                udpServer.Send([1], 1, remoteEp); // reply back
            }
            udpServer.Dispose();
        });


        // Act  
        var client = new UdpClient();
        var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port); // endpoint where server is listening
        client.Connect(ep);

        while (!cts.IsCancellationRequested)
        {

            // send data
            client.Send([1, 2, 3, 4, 5], 5);

            // then receive data
            var receivedData = client.Receive(ref ep);

            Debug.Print($"Client: receive data from {ep}: {receivedData.Length} bytes");
        }

        // Assert

        client.Dispose();

    }

    [Test]
    public void Test2_UdpSocketWithHello_UdpMessagesSentToClient()
    {
        // Arrange 
        var cts = new CancellationTokenSource(2000);

        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        Task.Run(() =>
        {
            var udpServer = new UdpSocket("Server", "127.0.0.1", port, true);

            started.Set();

            while (!cts.IsCancellationRequested)
            {
                var data = udpServer.Receive(); // listen on port 11000
                udpServer.Send("Blabb"); // reply back
            }
            udpServer.Dispose();

        });

        started.WaitOne(2000);

        // Act  
        var client = new UdpSocket("Client", "127.0.0.1", port, false);

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send("Blubb");

            // then receive data
            client.Receive();
        }

        client.Dispose();

        // Assert
        Assert.Pass();
    }

    [Test]
    public void Test2_UdpSocketNoHello_UdpMessagesSentToClient()
    {
        // Arrange 
        var cts = new CancellationTokenSource(2000);

        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        Task.Run(() =>
        {
            var client = new UdpSocket("Client", "127.0.0.1", port, false);

            var isSet = false;

            while (!cts.IsCancellationRequested)
            {
                // send data
                client.Send("BlubbBlabb");

                //// then receive data
                //client.Receive();

                if (isSet)
                {
                    continue;
                }

                started.Set();
                isSet = true;
            }

            client.Dispose();

        });

        started.WaitOne(2000);

        // Act  
        var udpServer = new UdpSocket("Server", "127.0.0.1", port, true);

        

        while (!cts.IsCancellationRequested)
        {
            var data = udpServer.Receive(); // listen on port 11000
            //udpServer.Send("Blabb"); // reply back
        }
        udpServer.Dispose();

        



        // Assert
        Assert.Pass();
    }

    [Test]
    public void Test3_UdpSocket2WithNoHello_UdpMessagesSentToClient()
    {
        // Arrange 
        var cts = new CancellationTokenSource(2000);

        var port = TestDataHelper.GetRandomPort();

        Task.Run(() =>
        {
            var client = new UdpSocket2("Client", "127.0.0.1", port, false);

            Debug.Print("Client started...");

            while (!cts.IsCancellationRequested)
            {
                //// send data
                //client.Send("Blubb");

                // then receive data
                client.Receive();
            }

            client.Dispose();
        });

        // Act  
        var udpServer = new UdpSocket2("Server", "127.0.0.1", port, true);

        Debug.Print("Server started...");

        while (!cts.IsCancellationRequested)
        {
            udpServer.Send("Blabb"); // reply back
        }
        udpServer.Dispose();

        // Assert
        Assert.Pass();
    }

    [Test]
    public void Test4_UdpSocket2WithNoHelloClientStartedFirst_UdpMessagesSentToClient()
    {
        // Arrange 
        var cts = new CancellationTokenSource(2000);

        var port = TestDataHelper.GetRandomPort();

        AutoResetEvent started = new(false);

        Task.Run(() =>
        {
            var client = new UdpSocket2("Client", "127.0.0.1", port, false);

            Debug.Print("Client started...");

            started.Set();

            while (!cts.IsCancellationRequested)
            {
                //// send data
                //client.Send("Blubb");

                // then receive data
                client.Receive();
            }

            client.Dispose();
        });

        // 
        started.WaitOne(1000);

        // Act  
        var udpServer = new UdpSocket2("Server", "127.0.0.1", port, true);

        Debug.Print("Server started...");

        while (!cts.IsCancellationRequested)
        {
            udpServer.Send("Blabb"); // reply back
        }
        udpServer.Dispose();

        // Assert
        Assert.Pass();
    }


}