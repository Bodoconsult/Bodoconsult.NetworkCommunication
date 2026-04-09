// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

internal class SampleTests
{
    [Test]
    public void Test()
    {
        // Arrange 
        var cts = new CancellationTokenSource(2000);

        var port = 11001;

        Task.Run(() =>
        {
            var udpServer = new UdpSocket("Server", "127.0.0.1", port, true);

            while (!cts.IsCancellationRequested)
            {
                var data = udpServer.Receive(); // listen on port 11000
                udpServer.Send("Blabb"); // reply back
            }
            udpServer.Dispose();

        });

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
    public void Test2()
    {
        // Arrange 
        // Source - https://stackoverflow.com/q/20038943
        // Posted by Tono Nam, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-04-09, License - CC BY-SA 3.0

        var cts = new CancellationTokenSource(2000);

        var port = 11001;

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


}