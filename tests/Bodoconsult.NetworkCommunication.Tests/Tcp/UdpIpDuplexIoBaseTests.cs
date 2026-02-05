// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

public abstract class UdpIpDuplexIoBaseTests : BaseUdpTests
{
    /// <summary>
    /// Holds the duplex IO channel implementation (see <see cref="IDuplexIo"/>) to use
    /// </summary>
    protected IDuplexIo DuplexIo;

    [TearDown]
    public void TestCleanUp()
    {
        if (DuplexIo == null)
        {
            return;
        }

        DuplexIo.Dispose();
        var t = DuplexIo.DisposeAsync();
        t.AsTask().Wait(2000);
        Socket?.Dispose();
        Socket = null;

        //Server?.Dispose();
    }

    [OneTimeTearDown]
    public void TestDispose()
    {
        Server?.Dispose();
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
    public virtual void Send(IDataMessage message)
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


        Server.Send(data);

        if (data2 != null)
        {
            Server.Send(data2);
        }

        Wait.Until(() => MessageCounter == expectedCount);

        // Act
        DuplexIo.StopCommunication().Wait(1000);

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
            Assert.That(MessageCounter > 0);
        }

        Assert.That(MessageCounter, Is.EqualTo(expectedCount));
    }


    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  

        // Assert
        Assert.That(DuplexIo.DataMessagingConfig.SocketProxy, Is.Not.Null);
    }

    [Test]
    public void StartCommunication_ValidSetup_CommStarted()
    {
        // Arrange 

        // Act  
        DuplexIo.StartCommunication().Wait();

        // Assert
        Assert.That(DuplexIo.Receiver, Is.Not.Null);
        Assert.That(DuplexIo.Sender, Is.Not.Null);

        DuplexIo.StopCommunication();
    }

    [Test]
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
    public void SendMessage_MessageSWithoutDatablock_NotSent()
    {
        // Arrange
        var message = new SdcpDataMessage
        {
            MessageType = MessageTypeEnum.Sent,
        };

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        Assert.That(!IsDataMessageSentFired);
        Assert.That(IsDataMessageNotSentFired);
        Assert.That(!IsComDevCloseFired);
    }

    [Test]
    public void SendMessage_MessageS_Sent()
    {
        // Arrange
        var message = new SdcpDataMessage
        {
            MessageType = MessageTypeEnum.Sent,
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
    public void SendMessage_EncodingError_Fails()
    {
        // Arrange
        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(Socket, FakeSendPacketProcessEnum.EncodingError);

        var message = new ShouldCrashDataMessage
        {
            MessageType = MessageTypeEnum.Sent
        };

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
    public virtual void SendMessage_SocketError_Fails()
    {
        // Arrange
        var socket = new FakeUdpSocketProxy();
        socket.SenderThrowSocketException = true;

        DuplexIo = GetDuplexIoWithFakeEncodeDecoder(socket, FakeSendPacketProcessEnum.SocketError);

        var message = new SdcpDataMessage
        {
            MessageType = MessageTypeEnum.Sent,
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

    //[Test]
    //public void ReceiveMessageFromTower_MessageS()
    //{

    //    // Arrange
    //    var message = new SmdTowerDataMessage(SmdTower.TowerSn, 0x08, 's', MessageTypeEnum.Received);

    //    var data = TransportTestDataHelper.GetTestDataForCommand(message.Command);
    //    RunBasicTests(data, 1);

    //    // Assert
    //    Wait.Until(() => IsMessageReceivedFired, 2000);
    //    Assert.That(IsMessageReceivedFired);
    //    Assert.That(!IsMessageNotReceivedFired);
    //    Assert.That(!IsComDevCloseFired);
    //    Assert.That(!IsCorruptedMessageFired);
    //    Assert.That(!IsOnNotExpectedMessageReceivedFired);
    //}

}