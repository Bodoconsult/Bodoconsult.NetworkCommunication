// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Common.BusinessTransactions.Requests;
using IpBackend.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSampleTests.Backend.Adapter
{
    [TestFixture]
    internal class TncpIpDeviceTcpIpBusinessLogicAdapterTests
    {
        [Test]
        public void GetPaths_Channels1And3_ReturnsArray()
        {
            // Arrange 
            var request = new StartMessagingReportBusinessTransactionRequestData()
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
            var request = new StartMessagingReportBusinessTransactionRequestData()
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
    }
}
