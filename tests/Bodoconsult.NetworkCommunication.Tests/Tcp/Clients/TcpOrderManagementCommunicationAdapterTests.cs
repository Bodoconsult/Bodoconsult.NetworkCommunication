// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Factories;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Clients;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class TcpOrderManagementCommunicationAdapterTests : TcpOrderManagementCommunicationAdapterBaseTests
{
    private bool _isReceived;

    [SetUp]
    protected void TestSetup()
    {
        TcpIpClientTestHelper.InitServer(this);

        Debug.Print("Start TestSetup");

        BaseReset();

        ISocketProxyFactory socketProxyFactory = new ClientSocketProxyFactory();
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        IDuplexIoFactory duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        IMonitorLoggerFactoryFactory monitorLoggerFactoryFactory = new MonitorLoggerFactoryFactory(Globals.Instance);
        ILogDataFactory logDataFactory = TestDataHelper.LogDataFactory;
        IAppLoggerProxyFactory appLoggerFactory = new AppLoggerProxyFactory();
        IAppEventSourceFactory appEventSourceFactory = new FakeAppEventSourceFactory();

        ICommunicationHandlerFactory communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory, monitorLoggerFactoryFactory, logDataFactory, appLoggerFactory, appEventSourceFactory);
        IOutboundDataMessageFactory outboundDataMessageFactory = new SdcpOutboundDataMessageFactory();
        OrderManagementCommunicationAdapter = new OrderManagementCommunicationAdapter(DataMessagingConfig,
            communicationHandlerFactory, outboundDataMessageFactory);
    }


    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        
        // Act  
        
        // Assert
        Assert.That(OrderManagementCommunicationAdapter.DataMessagingConfig, Is.EqualTo(DataMessagingConfig));
        Assert.That(OrderManagementCommunicationAdapter.IsConnected, Is.EqualTo(false));
        Assert.That(OrderManagementCommunicationAdapter.DataMessagingConfig.CheckIfCommunicationIsOnlineDelegate, Is.Not.Null);
        Assert.That(OrderManagementCommunicationAdapter.DataMessagingConfig.RaiseComDevCloseRequestDelegate, Is.Not.Null);
        Assert.That(OrderManagementCommunicationAdapter.DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate, Is.Not.Null);
    }

    private void RaiseAppLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        MessageCounter++;
        _isReceived = true;
    }

    [Test]
    public void SendMessage_ValidMessage_MessageSent()
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

        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        OrderManagementCommunicationAdapter.ComDevInit();

        Assert.That(OrderManagementCommunicationAdapter.IsConnected);

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        Assert.That(IsDataMessageSentFired, Is.True);

        OrderManagementCommunicationAdapter.ComDevInit();
    }
}