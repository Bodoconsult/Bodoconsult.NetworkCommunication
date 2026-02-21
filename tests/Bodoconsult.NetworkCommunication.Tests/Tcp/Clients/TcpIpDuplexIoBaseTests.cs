// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Clients;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public abstract class TcpIpDuplexIoBaseTests : BaseTcpTests
{
    /// <summary>
    /// Holds the duplex IO channel implementation (see <see cref="IDuplexIo"/>) to use
    /// </summary>
    protected IDuplexIo DuplexIo;

    [TearDown]
    public void TestCleanUp()
    {
        if (DuplexIo != null)
        {
            DuplexIo.StopCommunication().Wait();
            DuplexIo.Dispose();
            var t = DuplexIo.DisposeAsync();
            t.AsTask().Wait(2000);
            DuplexIo = null;
        }

        if (Socket != null)
        {
            Socket.Dispose();
            Socket = null;
        }

        if (RemoteTcpIpDevice == null)
        {
            return;
        }
        RemoteTcpIpDevice?.Dispose();
        RemoteTcpIpDevice = null;
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <returns></returns>
    public virtual IDuplexIo GetDuplexIo(ISocketProxy socketProxy)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <param name="expectedResult">Current expected result from send process</param>
    /// <returns></returns>
    public virtual IDuplexIo GetDuplexIoWithFakeEncodeDecoder(ISocketProxy socketProxy, FakeSendPacketProcessEnum expectedResult)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send a message with the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual void Send(IOutboundDataMessage message)
    {
        DuplexIo.StartCommunication().Wait();

        DuplexIo.SendMessage(message).Wait();

        var task = Task.Run(() =>
        {
            var i = 0;
            while (i < 200)
            {
                AsyncHelper.Delay(5);
                i++;
            }

        });
        task.Wait();

        DuplexIo.StopCommunication().Wait();
    }

    public virtual void SendDataAndReceive(byte[] data, int expectedCount, byte[] data2 = null)
    {
        // Arrange
        DuplexIo.StartCommunication().Wait();

        RemoteTcpIpDevice.Send(data);

        if (data2 != null)
        {
            RemoteTcpIpDevice.Send(data2);
        }


        var tcs1 = new TaskCompletionSource<bool>();
        var t1 = tcs1.Task;

        // Start a background task that will complete tcs1.Task
        Task.Factory.StartNew(() =>
        {
            var cts = new CancellationTokenSource(5000);
            while (!cts.IsCancellationRequested)
            {
                if (MessageCounter >= expectedCount)
                {
                    tcs1.SetResult(true);
                    return;
                }
                Task.Delay(50, cts.Token);
            }

            tcs1.SetResult(false);
        });


        var result = t1.GetAwaiter().GetResult();

        // Start a background task that will complete tcs1.Task

        // Act
        DuplexIo.StopCommunication().Wait(1000);

        Assert.That(result);

        Debug.Print("Process done");
    }

    private void RunBasicTests(byte[] data, int expectedCount, byte[] data2 = null)
    {
        // Arrange and act
        SendDataAndReceive(data, expectedCount, data2);

        // Assert
        if (expectedCount == 0)
        {
            Assert.That(MessageCounter, Is.EqualTo(expectedCount));
        }
        else
        {
            Assert.That(MessageCounter, Is.GreaterThan(0));
        }

        Assert.That(MessageCounter, Is.EqualTo(expectedCount));
    }

    [Test]
    [NonParallelizable]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  

        // Assert
        Assert.That(DuplexIo.DataMessagingConfig.SocketProxy, Is.Not.Null);
    }

    [Test]
    [NonParallelizable]
    public void StartCommunication_ValidSetup_Started()
    {
        // Arrange 

        // Act  
        DuplexIo.StartCommunication().Wait();

        // Assert
        Assert.That(DuplexIo.Receiver, Is.Not.Null);
        Assert.That(DuplexIo.Sender, Is.Not.Null);
    }

    [Test]
    [NonParallelizable]
    public void StopCommunication_ValidSetup_CommStopped()
    {
        // Arrange 

        // Act  
        DuplexIo.StartCommunication().Wait();
        DuplexIo.StopCommunication().Wait();

        // Assert
        Assert.That(DuplexIo.Receiver, Is.Not.Null);
        Assert.That(DuplexIo.Sender, Is.Not.Null);
        Assert.That(DuplexIo.Receiver.FillPipelineTask, Is.Null);
        Assert.That(DuplexIo.Receiver.SendPipelineTask, Is.Null);
    }

    [Test]
    [NonParallelizable]
    public void SendMessage_MessageSWithoutDatablock_NotSent()
    {
        // Arrange

        var message = new SdcpOutboundDataMessage();

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        Assert.That(!IsDataMessageSentFired);
        Assert.That(IsDataMessageNotSentFired);
        Assert.That(!IsComDevCloseFired);
    }

    [Test]
    [NonParallelizable]
    public void SendMessage_MessageS_Sent()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new SdcpDummyDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }
        };

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        Assert.That(IsDataMessageSentFired);
        Assert.That(!IsDataMessageNotSentFired);
        Assert.That(!IsComDevCloseFired);
    }

    [Test]
    [NonParallelizable]
    public void SendMessage_EncodingError_Fails()
    {
        // Arrange
        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(Socket, FakeSendPacketProcessEnum.EncodingError);

        var message = new ShouldCrashOutboundDataMessage();

        // Act

        //Assert.Throws<MessageNotSentException>(() =>
        //{
        Send(message);

        //});

        // Assert
        Wait.Until(() => IsDataMessageNotSentFired, 2000);
        Assert.That(!IsDataMessageSentFired);
        Assert.That(IsDataMessageNotSentFired);
        Assert.That(!IsComDevCloseFired);
    }

    [Test]
    [NonParallelizable]
    public virtual void SendMessage_SocketError_Fails()
    {
        // Arrange
        var socket = new FakeUdpSocketProxy();
        socket.SenderThrowSocketException = true;

        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(socket, FakeSendPacketProcessEnum.SocketError);

        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new SdcpDummyDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }
        };

        // Act
        Send(message);

        // Assert
        Wait.Until(() => IsDataMessageNotSentFired);
        Assert.That(IsDataMessageSentFired, Is.False);
        Assert.That(IsDataMessageNotSentFired, Is.True);
        Assert.That(IsComDevCloseFired, Is.True);
    }

    [Test]
    [NonParallelizable]
    public void ReceiveMessageFromTower_MessageS()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new SdcpDummyDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
            }
        };

        RunBasicTests(message.DataBlock.Data.ToArray(), 1);

        // Assert
        Wait.Until(() => IsMessageReceivedFired, 2000);
        Assert.That(IsMessageReceivedFired);
        Assert.That(!IsMessageNotReceivedFired);
        Assert.That(!IsComDevCloseFired);
        Assert.That(!IsCorruptedMessageFired);
        Assert.That(!IsOnNotExpectedMessageReceivedFired);
    }
}