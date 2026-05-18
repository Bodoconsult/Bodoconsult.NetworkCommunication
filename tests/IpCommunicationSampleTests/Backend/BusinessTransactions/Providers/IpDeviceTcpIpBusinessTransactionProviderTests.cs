// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpBackend.Bll.BusinessTransactions.Providers;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using Moq;

namespace IpCommunicationSampleTests.Backend.BusinessTransactions.Providers;

[TestFixture]
internal class IpDeviceTcpIpBusinessTransactionProviderTests
{
    [Test]
    public void Constructor_Always_RegistersCorrectCreateDelegates()
    {
        // Act.
        var provider = CreateProvider();

        // Assert.
        var delegates = provider.CreateBusinessTransactionDelegates;

        using (Assert.EnterMultipleScope())
        {

            Assert.That(delegates, Is.Not.Null);
            Assert.That(delegates, Has.Count.EqualTo(4));

            foreach (var item in delegates)
            {
                var transaction = item.Value.Invoke();
                Assert.That(item.Key, Is.EqualTo(transaction.Id));
            }
        }
    }

    [Test]
    public void Transaction1_StartStreaming_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction201_StartStreaming();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StartMessaging));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction2_StopMessaging_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction202_StopMessaging();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StopMessaging));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    private static IpDeviceTcpIpBusinessTransactionProvider CreateProvider()
    {
        var adapter = new Mock<IIpDeviceTcpIpDeviceBusinessLogicAdapter>();
        var provider = new IpDeviceTcpIpBusinessTransactionProvider(adapter.Object);

        return provider;
    }
}