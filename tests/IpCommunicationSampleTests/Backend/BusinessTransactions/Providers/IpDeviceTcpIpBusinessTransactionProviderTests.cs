// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpBackend.Bll.BusinessTransactions;
using IpBackend.Bll.BusinessTransactions.Providers;
using IpBackend.Bll.Interfaces;
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
        var transaction = provider.Transaction1_StartStreaming();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(BackendBusinessTransactionCodes.StartStreaming));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction2_StopStreaming_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction2_StopStreaming();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(BackendBusinessTransactionCodes.StopStreaming));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction3_StartSnapshot_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction3_StartSnapshot();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(BackendBusinessTransactionCodes.StartSnapshot));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction4_StopSnapshot_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction4_StopSnapshot();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
          Assert.That(transaction.Id, Is.EqualTo(BackendBusinessTransactionCodes.StopSnapshot));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    private static IpDeviceTcpIpBusinessTransactionProvider CreateProvider()
    {
        var articleGroupDelegate = new Mock<IIpDeviceTcpIpDeviceBusinessLogicAdapter>();
        var provider = new IpDeviceTcpIpBusinessTransactionProvider(articleGroupDelegate.Object);

        return provider;
    }
}