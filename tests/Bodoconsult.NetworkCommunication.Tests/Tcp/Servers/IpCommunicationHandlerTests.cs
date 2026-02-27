// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Servers;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class IpCommunicationHandlerTests : TcpIpCommunicationHandlerBaseTests
{
    private bool _isReceived;

    [SetUp]
    protected void TestSetup()
    {
        TcpIpServerTestHelper.InitClient(this);

        Debug.Print("Start TestSetup");

        BaseReset();

        TcpIpServerTestHelper.InitSocket(this);

        DuplexIo = GetDuplexIo(Socket);

        Debug.Print("End TestSetup");

        _isReceived = false;
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <returns><see cref="IDuplexIo"/> instance to test</returns>
    public override IDuplexIo GetDuplexIo(ISocketProxy socketProxy)
    {
        Socket = socketProxy;
        BindDelegates();

        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        return new IpDuplexIo(DataMessagingConfig, sendPacketProcessFactory);
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <param name="expectedResult">Current expected result from send process</param>
    /// <returns></returns>
    public override IDuplexIo GetDuplexIoWithFakeEncodeDecoder(ISocketProxy socketProxy, FakeSendPacketProcessEnum expectedResult)
    {
        Socket = socketProxy;
        BindDelegates();

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory
        {
            TypeOfFakeSendPacketProcessEnum = expectedResult
        };
        return new IpDuplexIo(DataMessagingConfig, sendPacketProcessFactory);
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        // Act  

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);

        // Assert
        Assert.That(ch.DuplexIo, Is.SameAs(DuplexIo));
        Assert.That(ch.SocketProxy, Is.SameAs(Socket));
    }

    [Test]
    public void Disconnect_ValidSetup_Disconnected()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);

        Assert.That(ch.IsConnected, Is.True);

        // Act  
        ch.Disconnect();

        // Assert
        Assert.That(ch.IsConnected, Is.False);
    }

    private void RaiseAppLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        MessageCounter++;
        _isReceived = true;
    }

    [Test]
    public void Connect_ValidSetup_Reconnected()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);

        Assert.That(ch.IsConnected, Is.True);

        ch.Disconnect();
        Assert.That(ch.IsConnected, Is.False);

        // Act  
        ch.Connect();
        TcpIpServerTestHelper.InitClient(this);
        RemoteTcpIpDevice.Start();


        Wait.Until(() => ch.IsConnected);

        // Assert
        Assert.That(ch.IsConnected, Is.True);
    }

    [Test]
    public void OnReceivedMessage_ValidSetupNothingReceived_IsNotReceived()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);
        DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = RaiseAppLayerDataMessageReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = ch.OnReceivedMessage;

        Assert.That(ch.IsConnected, Is.True);

        // Act  
        Wait.Until(() => false, 1000);

        // Assert
        Assert.That(_isReceived, Is.False);
    }

    [Test]
    public void OnReceivedMessage_ValidSetupMessageReceived_IsReceived()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);
        DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = RaiseAppLayerDataMessageReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = ch.OnReceivedMessage;

        Assert.That(ch.IsConnected, Is.True);

        var message = new byte[] { 0x2, 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 };

        // Act
        SendDataAndReceive(message, 1);
        Wait.Until(() => _isReceived, 5000);

        // Assert
        Assert.That(_isReceived, Is.True);
    }

    [Test]
    public void SendMessage_ValidMessage_MessageSent()
    {
        // Arrange 
        var appEventSourceFactory = TestDataHelper.AppEventSourceFactory;

        var ch = new IpCommunicationHandler(DuplexIo, DataMessagingConfig, appEventSourceFactory);
        DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = RaiseAppLayerDataMessageReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = ch.OnReceivedMessage;

        Assert.That(ch.IsConnected, Is.True);

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
        Assert.That(IsDataMessageSentFired, Is.True);
    }



}