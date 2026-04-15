// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Servers;

public abstract class BaseUdpIpDuplexIoTests : BaseUdpTests
{
    ///// <summary>
    ///// Holds the duplex IO channel implementation (see <see cref="IDuplexIo"/>) to use
    ///// </summary>
    //protected IDuplexIo? DuplexIo;

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

        DataMessagingConfig = null;
        IpAddress = null;

        if (RemoteUdpDevice == null)
        {
            return;
        }
        RemoteUdpDevice?.Dispose();
        RemoteUdpDevice = null;
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

    ///// <summary>
    ///// Send a message with the <see cref="IDuplexIo"/> instance to test
    ///// </summary>
    ///// <param name="message">Current message to send</param>
    //public override void Send(IOutboundDataMessage message)
    //{
    //    ArgumentNullException.ThrowIfNull(DuplexIo);

    //    DuplexIo.StartCommunication().Wait();

    //    DuplexIo.SendMessage(message).Wait();

    //    var task = Task.Run(() =>
    //    {
    //        var i = 0;
    //        while (i < 200)
    //        {
    //            AsyncHelper.Delay(5);
    //            i++;
    //        }

    //    });
    //    task.Wait();

    //    DuplexIo.StopCommunication().Wait();
    //}


    public virtual void SendDataAndReceive(byte[] data, int expectedCount, byte[]? data2 = null)
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);
        ArgumentNullException.ThrowIfNull(DuplexIo);

        DuplexIo.StartCommunication().Wait();

        RemoteUdpDevice.Send(data);

        if (data2 != null)
        {
            RemoteUdpDevice.Send(data2);
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


    private void RunBasicTests(byte[] data, int expectedCount, byte[]? data2 = null)
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
            Assert.That(MessageCounter > 0);
        }

        Assert.That(MessageCounter, Is.EqualTo(expectedCount));
    }
    
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(DuplexIo);

        // Act  

        // Assert
        Assert.That(DuplexIo.DataMessagingConfig.SocketProxy, Is.Not.Null);
    }

    [Test]
    public void StartCommunication_ValidSetup_CommStarted()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(DuplexIo);

        // Act  
        DuplexIo.StartCommunication().Wait();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(DuplexIo.Receiver, Is.Not.Null);
            Assert.That(DuplexIo.Sender, Is.Not.Null);
        }

        DuplexIo.StopCommunication();
    }

    [Test]
    public void StopCommunication_ValidSetup_CommStopped()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(DuplexIo);

        // Act  
        DuplexIo.StartCommunication().Wait();
        DuplexIo.StopCommunication().Wait();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(DuplexIo.Receiver, Is.Not.Null);
            Assert.That(DuplexIo.Sender, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(DuplexIo.Receiver);
            Assert.That(DuplexIo.Receiver.FillPipelineTask, Is.Null);
            Assert.That(DuplexIo.Receiver.SendPipelineTask, Is.Null);
        }
    }

    [Test]
    public void SendMessage_MessageSWithoutDatablock_NotSent()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage();

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!IsDataMessageSentFired);
            Assert.That(IsDataMessageNotSentFired);
            Assert.That(!IsComDevCloseFired);
        }
    }

    [Test]
    public void SendMessage_SdcpMessage_Sent()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage
        {
            WaitForAcknowledgement = false,
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }, 
            RawMessageData = new byte[] { DeviceCommunicationBasics.Stx, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, DeviceCommunicationBasics.Etx }
        };

        // Act
        SendDataFromLocalToRemote([message], 1);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsDataMessageSentFired);
            Assert.That(!IsDataMessageNotSentFired);
            Assert.That(!IsComDevCloseFired);
        }
    }

    // ToDo : fix this test
    //[Explicit]
    [Test, CancelAfter(10000)]
    public void SendMessage_MultipleSdcpMessages_Sent(CancellationToken cancellationToken)
    {
        // Arrange
        var messages = new List<IOutboundDataMessage>();

        // Act
        for (var i = 0; i < 100; i++)
        {
            var message = new SdcpOutboundDataMessage
            {
                WaitForAcknowledgement = false,
                DataBlock = new BasicOutboundDatablock
                {
                    DataBlockType = 'x',
                    Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
                },
                RawMessageData = new byte[] { DeviceCommunicationBasics.Stx, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, DeviceCommunicationBasics.Etx }
            };

            messages.Add(message);
        }

        SendDataFromLocalToRemote(messages, messages.Count - 1, cancellationToken);

        Wait.Until(() => IsDataMessageSentFired);

        Trace.Flush();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsDataMessageSentFired);
            Assert.That(!IsDataMessageNotSentFired);
            Assert.That(!IsComDevCloseFired);
            ArgumentNullException.ThrowIfNull(RemoteUdpDevice);
            Assert.That(RemoteUdpDevice.ReceivedMessages.Count, Is.GreaterThan(1));
        }
    }

    [Test]
    public void SendMessage_EncodingError_Fails()
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(Socket);

        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(Socket, FakeSendPacketProcessEnum.EncodingError);

        var message = new ShouldCrashOutboundDataMessage();

        // Act
        Send(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Wait.Until(() => IsDataMessageNotSentFired, 2000);
            Assert.That(!IsDataMessageSentFired);
            Assert.That(IsDataMessageNotSentFired);
            Assert.That(!IsComDevCloseFired);
        }
    }

    [Test]
    public virtual void SendMessage_SocketError_Fails()
    {
        // Arrange
        var socket = new FakeUdpSocketProxy();
        socket.SenderThrowSocketException = true;

        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(socket, FakeSendPacketProcessEnum.SocketError);

        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }
        };

        // Act
        Send(message);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Wait.Until(() => IsDataMessageNotSentFired);
            Assert.That(IsDataMessageSentFired, Is.False);
            Assert.That(IsDataMessageNotSentFired, Is.True);
            Assert.That(IsComDevCloseFired, Is.True);
        }
    }

    //[Test]
    //public void ReceiveMessageFromdevice_MessageS()
    //{

    //    // Arrange
    //    var message = new SdcpOutboundDataMessage
    //    {
    //        DataBlock = new BasicOutboundDatablock
    //        {
    //            DataBlockType = 'x',
    //            Data = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
    //        }
    //    };

    //    RunBasicTests(message.DataBlock.Data.ToArray(), 1);

    //    // Assert
    //    Wait.Until(() => IsMessageReceivedFired, 2000);
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(IsMessageReceivedFired);
    //        Assert.That(!IsMessageNotReceivedFired);
    //        Assert.That(!IsComDevCloseFired);
    //        Assert.That(!IsCorruptedMessageFired);
    //        Assert.That(!IsOnNotExpectedMessageReceivedFired);
    //    }
    //}
}