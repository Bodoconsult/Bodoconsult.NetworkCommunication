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
    private bool _isStopped;
    private bool _isActivated;
    private bool _isReset;

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
        OrderManagementCommunicationAdapter = new IpCommunicationAdapter(DataMessagingConfig,
            communicationHandlerFactory, outboundDataMessageFactory);

        _isStopped = false;
        _isReset = false;
        _isActivated = false;
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

        OrderManagementCommunicationAdapter.ComDevClose();
    }

    [Test]
    public void ComDevInit_ValidSetup_InitSuccessful()
    {
        // Arrange 
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        OrderManagementCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        OrderManagementCommunicationAdapter.ComDevInit();

        // Assert
        Assert.That(OrderManagementCommunicationAdapter.IsConnected);
        Assert.That(OrderManagementCommunicationAdapter.IsCommunicationHandlerNotNull);

        Wait.Until(() => _isStopped);

        Assert.That(_isStopped);
        Assert.That(_isActivated, Is.True);

        OrderManagementCommunicationAdapter.ComDevClose();
    }

    [Test]
    public void ComDevClose_ValidSetup_CloseSuccessful()
    {
        // Arrange 
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate = ResetOutboundDataMessageFactoryDelegate;

        OrderManagementCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        OrderManagementCommunicationAdapter.ComDevInit();

        Assert.That(OrderManagementCommunicationAdapter.IsConnected, Is.True);

        Wait.Until(() => _isStopped);

        Assert.That(_isStopped);
        Assert.That(_isActivated, Is.True);

        OrderManagementCommunicationAdapter.ComDevClose();

        // Assert
        Assert.That(OrderManagementCommunicationAdapter.IsConnected, Is.False);

        Wait.Until(() => _isStopped);

        Assert.That(_isStopped);
        Assert.That(_isActivated, Is.True);
        Assert.That(_isReset, Is.True);
    }

    [Test]
    public void IsPingableAsync_Localhost_Pingable()
    {
        // Arrange 
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        OrderManagementCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        var result = OrderManagementCommunicationAdapter.IsPingableAsync().GetAwaiter().GetResult();

        // Assert
        Assert.That(result);
    }


    private void ResetOutboundDataMessageFactoryDelegate()
    {
        _isReset = true;
    }

    private void SetOrderProcessingStateDelegate(bool isActivated)
    {
        _isStopped = true;
        _isActivated = isActivated;
    }
}