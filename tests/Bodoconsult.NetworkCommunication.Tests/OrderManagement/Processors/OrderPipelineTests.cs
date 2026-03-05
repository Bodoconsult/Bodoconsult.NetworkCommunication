// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
public class OrderPipelineTests
{
    private readonly IAppDateService _dateTimeService = TestDataHelper.AppDateService;
    private readonly IRequestProcessorFactory _requestProcessorFactory;
    private readonly IAppLoggerProxy _appLogger = Globals.Instance.Logger;
    private const string LoggerId = "001234";
    private readonly AppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    private bool _isOrderProcessingFinishedDelegateFired;

    public OrderPipelineTests()
    {
        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();
        
        var device = TestDataHelper.CreateDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        _requestProcessorFactory = new RequestProcessorFactory(requestStepProcessorFactory, device);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _benchLogger.Dispose();
    }

    private void OrderProcessingFinishedDelegate(long orderid)
    {
        _isOrderProcessingFinishedDelegateFired = true;
    }

    [SetUp]
    public void TestSetup()
    {
        _isOrderProcessingFinishedDelegateFired = false;
    }

    [Test]
    public void RemoveCancelledOrdersFromExecutionQueue_NoOrder_DoesNotThrow()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        // Act and Assert
        Assert.DoesNotThrow(() =>
        {
            op.RemoveCancelledOrdersFromExecutionQueue();
        });

    }

    [Test]
    public void AddOrder_ValidOrder_OrderAdded()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);

        // Act 
        op.AddOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(1));
            Assert.That(op.IsWaitingOrdersEmpty, Is.False);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
            Assert.That(order.ExecutionState, Is.EqualTo(OrderState.Added));
        };

    }

    [Test]
    public void AddOrder_ValidPriorityOrder_Throws()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        // Act and assert
        Assert.Throws<ArgumentException>(() =>
        {
            op.AddOrder(order);
        });
    }

    [Test]
    public void AddOrder_ValidOrderCancelled_OrderAdded()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = false;
        order.IsCancelled = true;

        // Act 
        op.AddOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
            Assert.That(order.ExecutionState, Is.EqualTo(OrderState.Added));
        }

    }

    [Test]
    public void AddPriorityOrder_ValidPriorityOrder_OrderAdded()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        // Act 
        op.AddPriorityOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(1));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.False);
            Assert.That(order.ExecutionState, Is.EqualTo(OrderState.Added));
        }

    }

    [Test]
    public void AddPriorityOrder_ValidNonPriorityOrder_Throws()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = false;

        // Act and assert
        Assert.Throws<ArgumentException>(() =>
        {
            op.AddPriorityOrder(order);
        });

    }

    [Test]
    public void AddPriorityOrder_ValidPriorityOrderCancelled_OrderAdded()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;
        order.IsCancelled = true;

        // Act 
        op.AddPriorityOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
            Assert.That(order.ExecutionState, Is.EqualTo(OrderState.Added));
        }

    }

    [Test]
    public void DequeueOrder_ValidOrder_OrderDequeued()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = false;

        op.AddOrder(order);

        // Act 
        op.DequeueOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }

    }

    [Test]
    public void DequeueOrder_ValidPriorityOrder_OrderDequeued()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        op.AddPriorityOrder(order);

        // Act 
        op.DequeueOrder(order);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }
    }

    [Test]
    public void CancelWaitingOrders_ValidOrder_OrderCancelled()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = false;

        op.AddOrder(order);

        // Act 
        op.CancelWaitingOrders();

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }
    }

    [Test]
    public void CancelWaitingPriorityOrders_ValidOrder_OrderCancelled()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        op.AddPriorityOrder(order);

        // Act 
        op.CancelWaitingPriorityOrders();

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(1));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }
    }

    [Test]
    public void GetNextNonPriorityOrder_ValidOrder_OrderCancelled()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = false;

        op.AddOrder(order);

        op.CancelWaitingOrders();

        // Act 
        var nextOrder = op.GetNextNonPriorityOrder();

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(nextOrder, Is.Null);
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }
    }

    [Test]
    public void GetNextPriorityOrder_ValidOrder_OrderCancelled()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        op.AddPriorityOrder(order);

        op.CancelWaitingPriorityOrders();

        // Act 
        var nextOrder = op.GetNextPriorityOrder();

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(nextOrder, Is.Null);
            Assert.That(op.AllWaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingOrdersEmpty, Is.True);
            Assert.That(op.AllWaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.WaitingPriorityOrders.Count, Is.EqualTo(0));
            Assert.That(op.IsWaitingPriorityOrdersEmpty, Is.True);
        }
    }

    [Test]
    public void PrepareOrderStart_ValidOrder_RequestProcessorIsCreatedCorrectly()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        // Act 
        var success = op.PrepareOrderStart(order, OrderProcessingFinishedDelegate, out var rp);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.False);
            Assert.That(rp, Is.Not.Null);
            Assert.That(rp.Order, Is.EqualTo(order));
            Assert.That(rp.OrderProcessingFinishedDelegate, Is.Not.Null);
        }
    }

    [Test]
    public void ExecuteOrder_ValidOrder_TaskIsStarted()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        var success = op.PrepareOrderStart(order, OrderProcessingFinishedDelegate, out var rp);
        Assert.That(success, Is.False);

        // Act 
        op.ExecuteOrder(rp);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(rp.CurrentTask, Is.Not.Null);
            //Assert.That(rp.Order.ExecutionState, Is.EqualTo(OrderState.Started));
        }

        rp.Cancel(true, false);
    }

    [Test]
    public void GetFromExecutionQueue_ValidOrder_RequestProcessorIsRemoved()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        var success = op.PrepareOrderStart(order, OrderProcessingFinishedDelegate, out var rp);

        Assert.That(success, Is.False);
        Assert.That(rp, Is.Not.Null);

        op.ExecuteOrder(rp);

        // Act 
        var rp2 = op.GetFromExecutionQueue(order.Id);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(rp2, Is.Not.Null);
            Assert.That(op.IsRunningOrdersEmpty, Is.False);
            Assert.That(_isOrderProcessingFinishedDelegateFired, Is.False);
        }

        rp.Cancel(true, false);
    }

    [Test]
    public void RemoveFromExecutionQueue_ValidOrder_RequestProcessorIsRemoved()
    {
        // Arrange 
        var op = new OrderPipeline(_dateTimeService, _requestProcessorFactory, _appLogger, LoggerId);

        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var order = TestDataHelper.CreateSdcpOrder(ps);
        order.IsHighPriorityOrder = true;

        var success = op.PrepareOrderStart(order, OrderProcessingFinishedDelegate, out var rp);

        Assert.That(success, Is.False);
        Assert.That(rp, Is.Not.Null);

        op.ExecuteOrder(rp);

        // Act 
        op.RemoveFromExecutionQueue(order.Id);

        // Assert
        using(Assert.EnterMultipleScope())
        {
            Assert.That(op.IsRunningOrdersEmpty, Is.True);
        }

        rp.Cancel(true, false);
    }
}