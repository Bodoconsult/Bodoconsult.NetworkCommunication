// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Clients;

public abstract class BaseUdpIpDuplexIoTests : BaseUdpTests
{
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

        if (RemoteUdpDevice == null)
        {
            return;
        }
        RemoteUdpDevice.Dispose();
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
    public void SendMessage_SdcpMessageWithoutDatablock_NotSent()
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
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }
        };

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsDataMessageSentFired);
            Assert.That(!IsDataMessageNotSentFired);
            Assert.That(!IsComDevCloseFired);
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

        //Assert.Throws<MessageNotSentException>(() =>
        //{
        Send(message);

        //});

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
        Wait.Until(() => IsDataMessageNotSentFired);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsDataMessageSentFired, Is.False);
            Assert.That(IsDataMessageNotSentFired, Is.True);
            Assert.That(IsComDevCloseFired, Is.True);
        }
    }

    [Test]
    public void ReceiveMessage_SdcpMessage_MessageReceived()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
            },
            RawMessageData = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
        };

        RunBasicReceiveTests([message.DataBlock.Data.ToArray()], 1);

        // Assert
        Wait.Until(() => IsMessageReceivedFired, 2000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsMessageReceivedFired);
            Assert.That(!IsMessageNotReceivedFired);
            Assert.That(!IsComDevCloseFired);
            Assert.That(!IsCorruptedMessageFired);
            Assert.That(!IsOnNotExpectedMessageReceivedFired);
        }
    }

    [Test]
    public void ReceiveMessage_TwoSdcpMessages_MessagesReceived()
    {
        // Arrange
        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
            },
            RawMessageData = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
        };

        var message2 = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x2, 0x79, 0x41, 0x6c, 0x75, 0x62, 0x62, 0x3 }
            },
            RawMessageData = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
        };

        RunBasicReceiveTests([message.DataBlock.Data.ToArray(), message2.DataBlock.Data.ToArray()], 2);

        // Assert
        Wait.Until(() => IsMessageReceivedFired, 2000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsMessageReceivedFired);
            Assert.That(!IsMessageNotReceivedFired);
            Assert.That(!IsComDevCloseFired);
            Assert.That(!IsCorruptedMessageFired);
            Assert.That(!IsOnNotExpectedMessageReceivedFired);
        }
    }

    [Test]
    public void ReceiveMessage_MultipleSdcpMessages_MessagesReceived()
    {
        // Arrange
        var messages = new List<byte[]>();

        for (var i = 0; i < 100; i++)
        {
            var message = new SdcpOutboundDataMessage
            {
                DataBlock = new BasicOutboundDatablock
                {
                    DataBlockType = 'x',
                    Data = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
                },
                RawMessageData = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 }
            };

            messages.Add(message.DataBlock.Data.ToArray());
        }

        RunBasicReceiveTests(messages, messages.Count);

        // Assert
        Wait.Until(() => IsMessageReceivedFired, 5000);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IsMessageReceivedFired);
            Assert.That(!IsMessageNotReceivedFired);
            Assert.That(!IsComDevCloseFired);
            Assert.That(!IsCorruptedMessageFired);
            Assert.That(!IsOnNotExpectedMessageReceivedFired);
        }
    }
}