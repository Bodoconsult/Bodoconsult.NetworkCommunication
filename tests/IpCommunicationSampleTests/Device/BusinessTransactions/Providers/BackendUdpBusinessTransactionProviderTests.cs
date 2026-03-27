// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Device.Bll.BusinessTransactions;
using IpCommunicationSample.Device.Bll.BusinessTransactions.Providers;
using IpCommunicationSample.Device.Bll.Interfaces;
using Moq;

namespace IpCommunicationSampleTests.Device.BusinessTransactions.Providers
{
    [TestFixture]
    internal class BackendUdpBusinessTransactionProviderTests
    {
        [Test]
        public void Constructor_Always_RegistersCorrectCreateDelegates()
        {
            // Act.
            var provider = CreateProvider();

            // Assert.
            var delegates = provider.CreateBusinessTransactionDelegates;
            Assert.That(delegates, Is.Not.Null);
            Assert.That(delegates, Has.Count.EqualTo(6));

            foreach (var item in delegates)
            {
                var transaction = item.Value.Invoke();
                Assert.That(item.Key, Is.EqualTo(transaction.Id));
            }
        }

        [Test]
        public void Transaction1_StartCommunication_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction1_StartCommunication();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StartCommunication));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Transaction2_StopCommunication_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction2_StopCommunication();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StopCommunication));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Transaction3_StartStreaming_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction3_StartStreaming();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StartStreaming));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Transaction4_StopStreaming_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction4_StopStreaming();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StopStreaming));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Transaction5_StartSnapshot_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction5_StartSnapshot();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StartSnapshot));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void Transaction6_StopSnapshot_Always_ReturnsCorrectTransaction()
        {
            // Arrange
            var provider = CreateProvider();

            // Act
            var transaction = provider.Transaction6_StopSnapshot();

            // Assert
            Assert.That(transaction, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(transaction.Id, Is.EqualTo(IpDeviceBusinessTransactionCodes.StopSnapshot));
                Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
                Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
            });
        }

        private static BackendUdpBusinessTransactionProvider CreateProvider()
        {
            var articleGroupDelegate = new Mock<IBackendUdpBusinessLogicAdapter>();
            var provider = new BackendUdpBusinessTransactionProvider(articleGroupDelegate.Object);

            return provider;
        }
    }
}
