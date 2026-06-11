// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Devices;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpDevice.Bll.BusinessLogic.Adapters;
using System.Diagnostics;

namespace IpCommunicationSampleTests.Device.Adapter;

internal class TncpBackendTcpIpBusinessLogicAdapterTests
{

    [Test]
    public void GetConfig_4Devices_ReturnsArray()
    {
        // Arrange 
        const string config = "1,2,3,4";

        // Act  
        var result = TncpBackendTcpIpBusinessLogicAdapter.GetConfig(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.EqualTo(254));
            Assert.That(result[0], Is.Zero);
            Assert.That(result[1], Is.EqualTo(1));
            Assert.That(result[2], Is.EqualTo(2));
            Assert.That(result[3], Is.EqualTo(3));
        }
    }

    [Test]
    public void GetConfig_3Devices_ReturnsArray()
    {
        // Arrange 
        const string config = "1,2,4";

        // Act  
        var result = TncpBackendTcpIpBusinessLogicAdapter.GetConfig(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.EqualTo(254));
            Assert.That(result[0], Is.Zero);
            Assert.That(result[1], Is.EqualTo(1));
            Assert.That(result[2], Is.EqualTo(3));
        }
    }

    [Test]
    public void CreateTncpReply_SetStreamOrder4Channels_ReturnsArray()
    {
        // Arrange 
        const string telnetCommand = "show,streamconfig";

        var adapter = new TncpBackendTcpIpBusinessLogicAdapter(
            new FakeSimpleDevice(TestDataHelper.GetDataMessagingConfig(),
                new DoNothingOrderManagementClientNotificationManager()), new FakeBusinessTransactionManager());

        // Act  
        var result = adapter.CreateTncpReply(telnetCommand);

        // Assert
        Assert.That(result.Length, Is.Not.Zero);

        Debug.Print(result.ToString());
    }
}