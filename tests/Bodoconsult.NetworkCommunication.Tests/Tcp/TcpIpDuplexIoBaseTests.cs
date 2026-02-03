// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

public abstract class TcpIpDuplexIoBaseTests : BaseTcpTests
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
    public void StartCommunication_ValidSetup_Started()
    {
        // Arrange 

        // Act  
        DuplexIo.StartCommunication().Wait();

        // Assert
        Assert.That(DuplexIo.Receiver, Is.Not.Null);
        Assert.That(DuplexIo.Sender, Is.Not.Null);

        DuplexIo.StopCommunication();

    }


    //[Test]
    //public void SendMessage_MessageS_Success()
    //{
    //    // Arrange
    //    var message = new SmdTowerDataMessage(SmdTower.TowerSn, 0x09, 's', MessageTypeEnum.Sent);

    //    // Act
    //    Send(message);

    //    Wait.Until(() => IsTowerMessageSentFired);

    //    // Assert
    //    Assert.That(IsTowerMessageSentFired);
    //    Assert.That(!IsTowerMessageNotSentFired);
    //    Assert.That(!IsComDevCloseFired);
    //}



    //[Test]
    //public void SendMessage_EncodingError_Fails()
    //{
    //    // Arrange
    //    DuplexIo = GetDuplexIoWithFakeEncodeDecoder(Socket, FakeSendPacketProcessEnum.EncodingError);

    //    var message = new SmdTowerDataMessage(SmdTower.TowerSn, 0x09, 's', MessageTypeEnum.Sent)
    //    {
    //        BlockAndRc = 0x7f
    //    };

    //    // Act

    //    //Assert.Throws<MessageNotSentException>(() =>
    //    //{
    //    Send(message);

    //    //});

    //    // Assert
    //    Wait.Until(() => IsTowerMessageNotSentFired, 2000);
    //    Assert.That(!IsTowerMessageSentFired);
    //    Assert.That(IsTowerMessageNotSentFired);
    //    Assert.That(!IsComDevCloseFired);
    //}

    //[Test]
    //public void SendMessage_SocketError_Fails()
    //{
    //    // Arrange
    //    DuplexIo = GetDuplexIoWithFakeEncodeDecoder(Socket, FakeSendPacketProcessEnum.SocketError);

    //    var message = new SmdTowerDataMessage(SmdTower.TowerSn, 0x09, 's', MessageTypeEnum.Sent);

    //    // Act

    //    //Assert.Throws<MessageNotSentException>(() =>
    //    //{
    //    Send(message);

    //    //});

    //    // AssertXmlAction_
    //    Wait.Until(() => IsTowerMessageNotSentFired);
    //    Assert.That(!IsTowerMessageSentFired);
    //    Assert.That(IsTowerMessageNotSentFired);
    //    Assert.That(IsComDevCloseFired);
    //}

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