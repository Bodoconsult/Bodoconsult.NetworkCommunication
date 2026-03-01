// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
public class SyncOrderManagerTests
{
    private readonly AppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    [OneTimeTearDown]
    public void Cleanup()
    {
        _benchLogger.Dispose();
    }

    [Test]
    public void AddSyncExecutionOrder_ValidOrder_OrderIsAddedToSyncQueue()
    {
        // Arrange 
        var op = new SyncOrderManager();

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = new SdcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());
        order.IsHighPriorityOrder = true;

        // Act 
        var result = op.AddSyncExecutionOrder(order.Id, 1000);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CancellationTokenSource, Is.Not.Null);
            Assert.That(result.TaskCompletionSource, Is.Null);
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(op.IsSyncRunningOrderEmpty, Is.False);
        }
    }

    [Test]
    public void RemoveSyncExecutionOrder_ValidOrder_OrderIsAddedToSyncQueue()
    {
        // Arrange 
        var op = new SyncOrderManager();

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = new SdcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());
        order.IsHighPriorityOrder = true;

        var result = op.AddSyncExecutionOrder(order.Id, 1000);

        using(Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CancellationTokenSource, Is.Not.Null);
            Assert.That(result.TaskCompletionSource, Is.Null);
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(op.IsSyncRunningOrderEmpty, Is.False);
        }

        // Act 
        op.RemoveSyncExecutionOrder(order.Id);

        // Assert
        Assert.That(op.IsSyncRunningOrderEmpty, Is.True);
    }
}