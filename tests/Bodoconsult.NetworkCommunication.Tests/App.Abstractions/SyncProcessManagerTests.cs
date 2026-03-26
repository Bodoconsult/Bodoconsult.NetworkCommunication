// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.NetworkCommunication.App.Abstractions.SyncProcessManager;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.App.Abstractions;

[TestFixture]
public class SyncProcessManagerTests
{
    private readonly AppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    [OneTimeTearDown]
    public void Cleanup()
    {
        _benchLogger.Dispose();
    }

    [Test]
    public void AddSyncProcess_ValidOrder_ProcessIsAddedToSyncQueue()
    {
        // Arrange 
        var op = new SyncProcessManager<DummyClass>();

        var processId = Guid.NewGuid();

        // Act 
        var result = op.AddSyncProcess(processId, 1000);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CancellationTokenSource, Is.Not.Null);
            Assert.That(result.TaskCompletionSource, Is.Null);
            Assert.That(result.ProcessId, Is.EqualTo(processId));
            Assert.That(op.IsSyncRunningOrderEmpty, Is.False);
        }
    }

    [Test]
    public void GetSyncProcessDataForProcess_ValidOrder_ReturnsData()
    {
        // Arrange 
        var op = new SyncProcessManager<DummyClass>();

        var processId = Guid.NewGuid();

        var dummyData = op.AddSyncProcess(processId, 1000);

        // Act 
        var result = op.GetSyncProcessDataForProcess(processId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CancellationTokenSource, Is.Not.Null);
            Assert.That(result.TaskCompletionSource, Is.Null);
            Assert.That(result.ProcessId, Is.EqualTo(processId));
            //Assert.That(op.IsSyncRunningOrderEmpty, Is.False);

            Assert.That(result, Is.EqualTo(dummyData));
        }
    }

    [Test]
    public void RemoveSyncProces_ValidOrder_ProcessIsRemovedFromSyncQueue()
    {
        // Arrange 
        var op = new SyncProcessManager<DummyClass>();

        var processId = Guid.NewGuid();

        var result = op.AddSyncProcess(processId, 1000);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CancellationTokenSource, Is.Not.Null);
            Assert.That(result.TaskCompletionSource, Is.Null);
            Assert.That(result.ProcessId, Is.EqualTo(processId));
            Assert.That(op.IsSyncRunningOrderEmpty, Is.False);
        }

        // Act 
        op.RemoveSyncProcess(processId);

        // Assert
        Assert.That(op.IsSyncRunningOrderEmpty, Is.True);
    }
}