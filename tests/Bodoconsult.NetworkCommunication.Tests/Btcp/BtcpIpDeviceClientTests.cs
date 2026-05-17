// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.ClientNotifications;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

internal class BtcpIpDeviceClientTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.Logger;

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        // Act  
        var client = new BtcpIpDeviceClient(device, _appLogger);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.Device, Is.EqualTo(device));
            Assert.That(client.AllowedNotifications, Is.Not.Null);
            Assert.That(client.AllowedNotifications.Count, Is.Not.Zero);
            Assert.That(client.AllowedNotifications.Contains(nameof(StateMachineStateNotification)), Is.True);
        }
    }

    [Test]
    public void LoadClientManager_ValidSetup_ClientManagerLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService(Globals.Instance);
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);

        // Act  
        client.LoadClientManager(clientManager);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.ClientManager, Is.EqualTo(clientManager));
        }
    }

    [Test]
    public void CheckNotification_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService(Globals.Instance);
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        // Act  
        var result = client.CheckNotification(noti);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void DoNotifyClient_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var commAdapter = new FakeIpCommunicationAdapter();
        var device = TestDataHelper.CreateSimpleDevice();
        device.LoadCommAdapter(commAdapter);

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new TestInboundBtcpMessageToBtRequestDataConverter(_appLogger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new TestInboundBtcpMessageToBtReplyConverter(_appLogger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new TestBtRequestDataToOutboundBtcpMessageConverter(_appLogger, Globals.Instance);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new TestBtReplyToOutboundDataMessageConverter(_appLogger);
        var btm = new FakeBusinessTransactionManager();

        IDeviceBusinessLogicAdapter adapter = new TestBtcpClientTcpIpBusinessLogicAdapter(device, btm, inboundDataMessageToBtRequestConverter,
            inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);
        device.LoadDeviceBusinessLogicAdapter(adapter);

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService(Globals.Instance);
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        var result = client.CheckNotification(noti);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            Assert.DoesNotThrow(() =>
            {
                client.DoNotifyClient(noti);
            });

            // Assert
            Assert.That(result, Is.True);
            Assert.That(commAdapter.WasSent, Is.True);
        }
    }

    [Test]
    public void ClientManager_DoNotifyAllClients_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var commAdapter = new FakeIpCommunicationAdapter();
        var device = TestDataHelper.CreateSimpleDevice();
        device.LoadCommAdapter(commAdapter);

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new TestInboundBtcpMessageToBtRequestDataConverter(_appLogger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new TestInboundBtcpMessageToBtReplyConverter(_appLogger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new TestBtRequestDataToOutboundBtcpMessageConverter(_appLogger, Globals.Instance);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new TestBtReplyToOutboundDataMessageConverter(_appLogger);
        var btm = new FakeBusinessTransactionManager();

        IDeviceBusinessLogicAdapter adapter = new TestBtcpClientTcpIpBusinessLogicAdapter(device, btm, inboundDataMessageToBtRequestConverter,
            inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);
        device.LoadDeviceBusinessLogicAdapter(adapter);

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService(Globals.Instance);
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            Assert.DoesNotThrow(() =>
            {
                clientManager.DoNotifyAllClients(noti);
            });

            // Assert
            Wait.Until(() => commAdapter.WasSent);
            Assert.That(commAdapter.WasSent, Is.True);
        }
    }
}