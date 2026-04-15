// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Infrastructure;
using System.Diagnostics;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Clients;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class IpCommunicationAdapterTests : BaseIpCommunicationAdapterTests
{
    private bool _isStopped;
    private bool _isActivated;
    private bool _isReset;

    [SetUp]
    protected void TestSetup()
    {
        TcpIpClientTestHelper.InitServer(this);

        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        Debug.Print("Start TestSetup");

        BaseReset();

        ISocketProxyFactory socketProxyFactory = new SocketProxyFactory(null);
        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        IDuplexIoFactory duplexIoFactory = new IpDuplexIoFactory(sendPacketProcessFactory);
        IAppEventSourceFactory appEventSourceFactory = new FakeAppEventSourceFactory();

        ICentralClientNotificationManager clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();

        ICommunicationHandlerFactory communicationHandlerFactory = new IpCommunicationHandlerFactory(socketProxyFactory, duplexIoFactory, appEventSourceFactory, clientNotificationManager);

        IpCommunicationAdapter = new IpCommunicationAdapter(DataMessagingConfig, communicationHandlerFactory);

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
        ArgumentNullException.ThrowIfNull(IpCommunicationAdapter);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IpCommunicationAdapter.DataMessagingConfig, Is.EqualTo(DataMessagingConfig));
            Assert.That(IpCommunicationAdapter.IsConnected, Is.EqualTo(false));
            Assert.That(IpCommunicationAdapter.DataMessagingConfig.CheckIfCommunicationIsOnlineDelegate, Is.Not.Null);
            Assert.That(IpCommunicationAdapter.DataMessagingConfig.RaiseComDevCloseRequestDelegate, Is.Not.Null);
            Assert.That(IpCommunicationAdapter.DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate, Is.Not.Null);
            Assert.That(IpCommunicationAdapter.DataMessagingConfig.CheckIfDeviceIsReadyDelegate, Is.Not.Null);
        }
    }

    [Test]
    public void SendMessage_ValidMessage_MessageSent()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(IpCommunicationAdapter);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        var message = new SdcpOutboundDataMessage
        {
            DataBlock = new BasicOutboundDatablock
            {
                DataBlockType = 'x',
                Data = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 }
            }
        };

        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        IpCommunicationAdapter.ComDevInit();

        Assert.That(IpCommunicationAdapter.IsConnected);

        // Act
        Send(message);

        Wait.Until(() => IsDataMessageSentFired);

        // Assert
        Assert.That(IsDataMessageSentFired, Is.True);

        IpCommunicationAdapter.ComDevClose();
    }

    [Test]
    public void ComDevInit_ValidSetup_InitSuccessful()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(IpCommunicationAdapter);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        IpCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        IpCommunicationAdapter.ComDevInit();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IpCommunicationAdapter.IsConnected);
            Assert.That(IpCommunicationAdapter.IsCommunicationHandlerNotNull);

            Wait.Until(() => _isStopped);

            Assert.That(_isStopped);
            Assert.That(_isActivated, Is.True);
        }

        IpCommunicationAdapter.ComDevClose();
    }

    [Test]
    public void ComDevClose_ValidSetup_CloseSuccessful()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(IpCommunicationAdapter);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate = ResetOutboundDataMessageFactoryDelegate;

        IpCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        IpCommunicationAdapter.ComDevInit();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(IpCommunicationAdapter.IsConnected, Is.True);

            Wait.Until(() => _isStopped);

            Assert.That(_isStopped);
            Assert.That(_isActivated, Is.True);

            IpCommunicationAdapter.ComDevClose();

            Assert.That(IpCommunicationAdapter.IsConnected, Is.False);

            Wait.Until(() => _isStopped);

            Assert.That(_isStopped);
            Assert.That(_isActivated, Is.True);
            Assert.That(_isReset, Is.True);
        }
    }

    [Test]
    public void IsPingableAsync_Localhost_Pingable()
    {
        // Arrange 
        ArgumentNullException.ThrowIfNull(IpCommunicationAdapter);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

        IpCommunicationAdapter.SetOrderProcessingStateDelegate = SetOrderProcessingStateDelegate;

        // Act
        var result = IpCommunicationAdapter.IsPingableAsync().GetAwaiter().GetResult();

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