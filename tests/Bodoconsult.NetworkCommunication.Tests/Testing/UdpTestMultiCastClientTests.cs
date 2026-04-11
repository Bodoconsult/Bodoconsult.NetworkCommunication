// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;
using System.Net;

namespace Bodoconsult.NetworkCommunication.Tests.Testing;

// ToDO check why this tests make the next tests fail

//[Explicit]
[TestFixture]
internal class UdpTestMultiCastServerTests
{
    [Test]
    public async Task SendReceive_PermanentMode_MessageReceived()
    {
        // Arrange 
        var serverData = new byte[] { 0x0, 0x1, 0x2 };
        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

        var ip = IPAddress.Parse("224.0.0.42");
        var port = TestDataHelper.GetRandomPort();
        var serverCount = 0;

        var cts = new CancellationTokenSource(5000);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var task = Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        {

            var udpServer = new UdpTestMultiCastServer(ip, port);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await udpServer.Receive(); // listen on port 11000
                    udpServer.Send(serverData); // reply back
                }

                serverCount = udpServer.ReceivedMessages.Count;

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

        await Task.Delay(100);

        // Act  
        var client = new UdpTestMultiCastClient(ip, port);
        //client.Start();

        while (!cts.IsCancellationRequested)
        {
            // send data
            client.Send(clientData);

            // then receive data
            await client.Receive();
        }

        client.Dispose();

        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
        task.Wait();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(3));
            Assert.That(serverCount, Is.GreaterThan(3));
        }
    }

    [Test]
    public void SendReceive_SingleMessages_MessageReceived()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x1, 0x2 };

        var ip = IPAddress.Parse("224.0.0.42");
        var port = TestDataHelper.GetRandomPort();

        var server = new UdpTestMultiCastServer(ip, port);
        server.Start();

        var client = new UdpTestMultiCastClient(ip, port);
        client.Start();
        client.Send(data);

        Wait.Until(() => !server.ReceivedMessages.IsEmpty);

        // Act  
        server.Send(data);

        Wait.Until(() => false, 500);

        // Assert
        Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));


        client.Dispose();

        var success = client.ReceivedMessages.TryTake(out var msg);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(msg.Length, Is.EqualTo(data.Length));
            Assert.That(msg.Span[0], Is.EqualTo(data[0]));
            Assert.That(msg.Span[1], Is.EqualTo(data[1]));
            Assert.That(msg.Span[2], Is.EqualTo(data[2]));
        }
    }

//    [Test]
//    public async Task SendReceive_PermanentMode_MessageReceived()
//    {
//        Arrange
//       var serverData = new byte[] { 0x0, 0x1, 0x2 };
//        var clientData = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };

//        var ip = IPAddress.Parse("224.0.0.42");
//        var port = TestDataHelper.GetRandomPort();
//        var serverCount = 0;

//        var cts = new CancellationTokenSource(5000);

//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//        ReSharper disable once MethodSupportsCancellation
//        Task.Run(async () =>
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//        {
//            try
//            {
//                var udpServer = new UdpTestMultiCastServer(ip, port);
//                udpServer.Start();

//                while (!cts.IsCancellationRequested)
//                {
//                    await udpServer.Receive(); // listen
//                    udpServer.Send(serverData); // reply back
//                }

//                serverCount = udpServer.ReceivedMessages.Count;
//                udpServer.Dispose();
//            }
//            catch (Exception e)
//            {
//                Debug.Print(e.ToString());
//                throw;
//            }

//        });

//        await Task.Delay(100, cts.Token);

//        Act
//       var client = new UdpTestMultiCastClient(ip, port);
//        client.Start();

//        while (!cts.IsCancellationRequested)
//        {
//            send data
//            client.Send(clientData);

//            then receive data
//           await client.Receive();
//        }

//        client.Dispose();

//        Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(3));
//            Assert.That(serverCount, Is.GreaterThan(3));
//        }
//    }

//    [Test]
//    public void SendReceive_SingleMessages_MessageReceived()
//    {
//        Arrange
//       var data = new byte[] { 0x0, 0x1, 0x2 };

//        var ip = IPAddress.Parse("224.0.0.42");
//        var port = TestDataHelper.GetRandomPort();

//        var server = new UdpTestMultiCastServer(ip, port);
//        server.Start();

//        var client = new UdpTestMultiCastClient(ip, port);
//        client.Start();
//        client.Send(data);

//        Act
//        server.Send(data);

//        Wait.Until(() => false, 500);

//        Assert
//        Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));

//        server.Dispose();
//        client.Dispose();

//        var success = client.ReceivedMessages.TryTake(out var msg);

//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(success, Is.True);
//            Assert.That(msg.Length, Is.EqualTo(data.Length));
//            Assert.That(msg.Span[0], Is.EqualTo(data[0]));
//            Assert.That(msg.Span[1], Is.EqualTo(data[1]));
//            Assert.That(msg.Span[2], Is.EqualTo(data[2]));
//        }
//    }


    //[Test]
    //public void SendReceive_ValidSetup_MessageReceived()
    //{
    //    // Arrange 
    //    var data = new byte[] { 0x0, 0x1, 0x2 };

    //    var ip = IPAddress.Parse("224.0.0.42");
    //    const int port = 65000;

    //    var server = new UdpTestMultiCastServer(ip, port);
    //    server.Start();

    //    var client = new UdpTestMultiCastClient(ip, port);
    //    client.Start();

    //    // Act  
    //    server.Send(data);

    //    Wait.Until(() => false, 500);

    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(client.ReceivedMessages.Count, Is.GreaterThan(0));

    //        var success = client.ReceivedMessages.TryTake(out var msg);

    //        Assert.That(success, Is.True);
    //        Assert.That(msg.Length, Is.EqualTo(data.Length));
    //        Assert.That(msg.Span[0], Is.EqualTo(data[0]));
    //        Assert.That(msg.Span[1], Is.EqualTo(data[1]));
    //        Assert.That(msg.Span[2], Is.EqualTo(data[2]));
    //    }

    //    server.Dispose();
    //    client.Dispose();
    //}

}