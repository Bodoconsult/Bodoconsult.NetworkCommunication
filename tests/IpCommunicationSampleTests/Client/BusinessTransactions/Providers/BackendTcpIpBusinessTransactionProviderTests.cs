// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpClient.Bll.BusinessTransactions.Providers;
using IpClient.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using Moq;

namespace IpCommunicationSampleTests.Client.BusinessTransactions.Providers;

[TestFixture]
internal class BackendTcpIpBusinessTransactionProviderTests
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
            Assert.That(delegates, Has.Count.EqualTo(6));

            foreach (var item in delegates)
            {
                var transaction = item.Value.Invoke();
                Assert.That(item.Key, Is.EqualTo(transaction.Id));
            }
        }
    }

    [Test]
    public void Transaction201_StartStreaming_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction201_StartStreaming();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StartStreaming));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction202_StopStreaming_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction202_StopStreaming();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StopStreaming));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction203_StartSnapshot_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction203_StartSnapshot();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StartSnapshot));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction204_StopSnapshot_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction204_StopSnapshot();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.StopSnapshot));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction250_CreateFftAnalysisReport_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction250_CreateFftAnalysisReport();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ClientSideBusinessTransactionIds.CreateFftAnalysisReport));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    private static BackendTcpIpBusinessTransactionProvider CreateProvider()
    {
        var articleGroupDelegate = new Mock<IBackendTcpIpBusinessLogicAdapter>();
        var provider = new BackendTcpIpBusinessTransactionProvider(articleGroupDelegate.Object);

        return provider;
    }
}