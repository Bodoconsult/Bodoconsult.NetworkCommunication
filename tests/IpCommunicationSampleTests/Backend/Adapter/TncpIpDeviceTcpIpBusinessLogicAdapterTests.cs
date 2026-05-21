// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Extensions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using IpBackend.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSampleTests.Backend.Adapter;

[TestFixture]
internal class TncpIpDeviceTcpIpBusinessLogicAdapterTests
{
    [Test]
    public void GetPaths_Channels1And3_ReturnsArray()
    {
        // Arrange 
        var request = new StartMessagingBusinessTransactionRequestData()
        {
            Snapshot = true,
            Channel1 = true,
            Channel3 = true
        };

        // Act  
        var result = TncpIpDeviceTcpIpBusinessLogicAdapter.GetPaths(request);

        // Assert
        Assert.That(result, Is.EqualTo("1,3"));
    }

    [Test]
    public void GetPaths_AllChannels_ReturnsArray()
    {
        // Arrange 
        var request = new StartMessagingBusinessTransactionRequestData()
        {
            Snapshot = true,
            Channel1 = true,
            Channel3 = true,
            Channel2 = true,
            Channel4 = true
        };

        // Act  
        var result = TncpIpDeviceTcpIpBusinessLogicAdapter.GetPaths(request);

        // Assert
        Assert.That(result, Is.EqualTo("1,2,3,4"));
    }

    [Test]
    public void GetConfig_ValidConfig_ReturnsArray()
    {
        // Arrange 
        const string cmd = "<CONFIG>0x10x20x30x40xC0xF";

        var expectedResult = new byte[] { 0x0, 0x1, 0x2, 0x3, 0xC, 0xF };

        // Act  
        var result = TncpIpDeviceTcpIpBusinessLogicAdapter.GetConfig(cmd);

        // Assert
        Assert.That(result.IsEqualTo(expectedResult), Is.True);
    }
}