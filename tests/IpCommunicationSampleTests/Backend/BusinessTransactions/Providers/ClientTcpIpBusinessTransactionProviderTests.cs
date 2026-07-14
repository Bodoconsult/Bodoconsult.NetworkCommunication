// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpBackend.Bll.BusinessTransactions.Providers;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using Moq;

namespace IpCommunicationSampleTests.Backend.BusinessTransactions.Providers;

[TestFixture]
internal class ClientTcpIpBusinessTransactionProviderTests
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
            Assert.That(delegates, Has.Count.EqualTo(2));

            foreach (var item in delegates)
            {
                var transaction = item.Value.Invoke();
                Assert.That(item.Key, Is.EqualTo(transaction.Id));
            }
        }
    }

    [Test]
    public void Transaction101_StartStreaming_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction101_ReportDeviceError();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ServerSideBusinessTransactionIds.ReportDeviceError));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void Transaction102_ReportFftData_Always_ReturnsCorrectTransaction()
    {
        // Arrange
        var provider = CreateProvider();

        // Act
        var transaction = provider.Transaction102_ReportFftData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Id, Is.EqualTo(ServerSideBusinessTransactionIds.ReportFftData));
            Assert.That(transaction.RunBusinessTransactionDelegate, Is.Not.Null);
            Assert.That(transaction.AllowedRequestDataTypes, Has.Count.EqualTo(1));
        }
    }

    private static ClientTcpIpBusinessTransactionProvider CreateProvider()
    {
        var adapter = new Mock<IClientTcpIpDeviceBusinessLogicAdapter>();
        var provider = new ClientTcpIpBusinessTransactionProvider(adapter.Object);

        return provider;
    }
}