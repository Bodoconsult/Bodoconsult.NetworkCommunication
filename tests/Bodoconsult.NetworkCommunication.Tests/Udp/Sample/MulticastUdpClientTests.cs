// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Sample;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class MulticastUdpClientTests
{
    [Test]
    public void SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("224.0.0.42");
        var port = TestDataHelper.GetRandomPort();

        var cts = new CancellationTokenSource(5000);

        Task.Run(() =>
        {
            try
            {
                var udpServer = new MulticastUdpClient(ip, port);
                //udpServer.Start();

                while (!cts.IsCancellationRequested)
                {
                    udpServer.Receive(); // listen
                    udpServer.Send(serverData); // reply back
                }

                udpServer.Dispose();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                throw;
            }

        });

        Task.Delay(100);

        // Act  
        var client = new MulticastUdpClient(ip, port);
        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send(clientData);

            // then receive data
            client.Receive();
        }

        client.Dispose();

        // Assert
        Assert.Pass();
    }
}