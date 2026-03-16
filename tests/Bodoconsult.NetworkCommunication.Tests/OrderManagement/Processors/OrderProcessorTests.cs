// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class OrderProcessorTests
{
    private OrderProcessor _processor;

    private readonly IList<IInboundDataMessage> _receivedMessage = new List<IInboundDataMessage>();

    private FakeIpCommunicationAdapter _commAdapter;

    private RequestStepProcessorFactory _stepFactory;

    private RequestProcessorFactory _orderProcessorFactory;

    private readonly IAppDateService _dateTimeService = TestDataHelper.AppDateService;

    private FakeNoStateMachineDevice _device;

    private readonly IAppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    private readonly IOrderManagementClientNotificationManager _notificationManager = new FakeOrderManagementClientNotificationManager();
    private bool _isBusinesDelegateFired;

    private readonly SdcpOrderBuilder _builder;
    private readonly TestOrderBuilder _testBuilder;
    private readonly LongRunningTestOrderBuilder _longRunningTestBuilder;
    private readonly ExtraLongRunningTestOrderBuilder _extraLongRunningTestBuilder;

    public OrderProcessorTests()
    {
        _builder = new SdcpOrderBuilder();
        _testBuilder = new TestOrderBuilder();
        _longRunningTestBuilder = new LongRunningTestOrderBuilder();
        _extraLongRunningTestBuilder = new ExtraLongRunningTestOrderBuilder();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _benchLogger.Dispose();
    }


    public void TestTearDown()
    {
        _processor?.Dispose();
        _receivedMessage.Clear();
    }


    #region Helper methods for faking receiver

    private IInboundDataMessage DecodeDataMessage(Memory<byte> message)
    {
        ArgumentNullException.ThrowIfNull(_device.DataMessagingConfig);
        ArgumentNullException.ThrowIfNull(_device.DataMessagingConfig.DataMessageProcessingPackage);

        var codecResult = _device.DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor.DecodeDataMessage(message);
        Assert.That(codecResult, Is.Not.Null);
        Assert.That(codecResult.ErrorCode, Is.Zero);

        return (IInboundDataMessage)codecResult.DataMessage;
    }


    private bool SendReceivedMessage()
    {
        if (_receivedMessage.Count == 0)
        {
            return true;
        }

        var msg = _receivedMessage[0];


        if (msg == null)
        {
            return true;
        }

        _receivedMessage.Remove(msg);

        AsyncHelper.Delay(25);

        Debug.Print($"TOPTests: Receiving message started");
        _processor.CheckReceivedMessage(msg);

        return false;
    }

    /// <summary>
    /// Inject a fake received message in the receiver thread
    /// </summary>
    /// <param name="receivedMessage"></param>
    private void ReceiveMessage(IInboundDataMessage receivedMessage)
    {
        // Debug.Print($"UTTOP: Received message with command {receivedMessage.Command}. Current message: {(_receivedMessage == null ? "null" : "not null")}");
        _receivedMessage?.Add(receivedMessage);
    }

    /// <summary>
    /// Create a SDCP order for testing
    /// </summary>
    /// <returns></returns>
    private IOrder CreateSdcpOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, _builder)
        {
            OrderId = 1,
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = _builder.CreateOrder(config);
        return order;
    }

    /// <summary>
    /// Create a test order for testing
    /// </summary>
    /// <returns></returns>
    private IOrder CreateTestOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, _testBuilder)
        {
            OrderId = 1,
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = _testBuilder.CreateOrder(config);
        return order;
    }

    /// <summary>
    /// Create a longrunning test order for testing
    /// </summary>
    /// <returns></returns>
    private IOrder CreateLongRunningTestOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, _longRunningTestBuilder)
        {
            OrderId = 1,
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = _longRunningTestBuilder.CreateOrder(config);
        return order;
    }

    /// <summary>
    /// Create an extra longrunning test order for testing
    /// </summary>
    /// <returns></returns>
    private IOrder CreateExtraLongRunningTestOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, _extraLongRunningTestBuilder)
        {
            OrderId = 1,
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = _extraLongRunningTestBuilder.CreateOrder(config);
        return order;
    }

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage message, object transportObject, IParameterSet parameterSet)
    {
        _isBusinesDelegateFired = true;
        return new MessageHandlingResult
        {
            ExecutionResult = OrderExecutionResultState.Successful
        };
    }


    private void PrepareTest(bool startOrderProcessing = true)
    {
        _isBusinesDelegateFired = false;
        _receivedMessage.Clear();

        // Test init
        _stepFactory = new RequestStepProcessorFactory();

        _device = TestDataHelper.CreateNoStateMachineDevice();
        _device.LoadCommAdapter(_commAdapter);

        var logger = Globals.Instance.Logger;
        _orderProcessorFactory = new RequestProcessorFactory(_stepFactory, _device);

        var orderPipeline = new OrderPipeline(_dateTimeService, _orderProcessorFactory, logger, "Tower 000123: ");
        var syncManager = new SyncOrderManager();

        var processor = new OrderProcessor(_device, _dateTimeService, orderPipeline, syncManager, _notificationManager, _benchLogger)
        {
            IsNoHardWareInitRequired = true
        };

        var om = new FakeOrderManager(_device.DataMessagingConfig, new FakeOrderProcessor(_device, new FakeOrderPipeline(), new SyncOrderManager(), new FakeOrderManagementClientNotificationManager()), new FakeOrderReceiver(), new OrderFactory())
        {
            OrderProcessor = processor
        };

        _device.LoadDeviceOrderManager(om);

        _processor = processor;

        //_towerOrderManager = new OrderManager(_commAdapter.SmdTower, _processor, new FakeTowerOrderReceiver(),
        //    _commAdapter.Logger);

        if (startOrderProcessing)
        {
            _processor.StartOrderProcessing();
        }
    }


    /// <summary>
    /// Set shorter timeouts for all process steps to avoid hanging test runs
    /// </summary>
    /// <param name="order"></param>
    /// <param name="timeOut"></param>
    private void SetTimeoutsAndDelegate(IOrder order, int timeOut = 723)
    {
        order.IsRunningSync = true;

        foreach (var spec in order.RequestSpecs)
        {
            if (spec is not DeviceRequestSpec drs)
            {
                continue;
            }

            drs.RequestAnswerStepIsStartedDelegate = SendReceivedMessage;

            foreach (var step in drs.RequestAnswerSteps)
            {
                if (step.Timeout < timeOut)
                {
                    step.Timeout = timeOut;
                }

                if (step.Timeout > timeOut)
                {
                    step.Timeout = timeOut;
                }
            }
        }
    }

    #endregion

    [Test]
    public void Ctor_ValidSetup_PropertiesSetupCorrectly()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        // Act  
        PrepareTest(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_processor.OrdersInProcessing.Count, Is.Zero);
            Assert.That(_processor.OrdersInQueue.Count, Is.Zero);
        }

        TestTearDown();

    }

    [Test]
    public void AddOrder_NewOrder_OrderAddedToQueue()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        PrepareTest(false);

        // Create parameter set and then the order

        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        // Act  
        _processor.AddOrder(order);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_processor.OrdersInQueue.Count, Is.EqualTo(1));
            Assert.That(_processor.IsAnyOrderToProcess, Is.True);
            Assert.That(_processor.InitInTheQueue, Is.False);
            Assert.That(_processor.IsNoOrderInProcessing, Is.True);
            Assert.That(_processor.OrdersInProcessingCount, Is.Zero);
        }

        TestTearDown();
    }


    [Test]
    public void AddOrder_DuplicatedOrder_OrderNotAddedToQueue()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        var order2 = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);
        SetTimeoutsAndDelegate(order2);

        // Act  
        _processor.AddOrder(order);
        _processor.AddOrder(order);
        _processor.AddOrder(order2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_processor.OrdersInQueue.Count, Is.EqualTo(2));
            Assert.That(_processor.IsAnyOrderToProcess, Is.True);
            Assert.That(_processor.InitInTheQueue, Is.False);
            Assert.That(_processor.IsNoOrderInProcessing, Is.True);
            Assert.That(_processor.OrdersInProcessingCount, Is.Zero);
        }

        TestTearDown();
    }

    [Test]
    public void Runner_NewOrderNoMessageReceived_OrderTimeout()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        //// Arrange: create received messages
        //var receivedMessage = new SdcpInboundDataMessage();
        //ReceiveMessage(receivedMessage);

        // Act
        _processor.AddOrder(order);
        _processor.Runner();

        // ReSharper disable once AccessToDisposedClosure
        Wait.Until(() => _processor.OrdersInProcessing.Count == 0, 3000);
        Wait.Until(() => order.ExecutionResult.Id == OrderExecutionResultState.Successful.Id, 3000);

        // Assert
        Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Timeout));

        TestTearDown();
    }

    [Test]
    public void Runner_NewOrder_OrderSuccessful()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        // Arrange: create received messages
        var receivedMessage = new SdcpInboundDataMessage();
        ReceiveMessage(receivedMessage);

        // Act
        _processor.AddOrder(order);
        _processor.Runner();

        // ReSharper disable once AccessToDisposedClosure
        Wait.Until(() => _processor.OrdersInProcessing.Count == 0, 3000);
        Wait.Until(() => order.ExecutionResult.Id == OrderExecutionResultState.Successful.Id, 3000);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
            Assert.That(_isBusinesDelegateFired, Is.True);
        }

        TestTearDown();
    }

    [Test]
    public void Cancel_NewOrderWith_OrderCancelled()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        _processor.AddOrder(order);
        _processor.Runner();

        // Act
        _processor.CancelOrder(order);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.ExecutionResult,
                Is.SameAs(OrderExecutionResultState.Unsuccessful).Or.SameAs(OrderExecutionResultState.Timeout));
            Assert.That(!order.WasSuccessful);
            Assert.That(order.IsCancelled);
        }

        TestTearDown();
    }

    //[Test]
    //public void Runner_NewOrderWithCancelRunningOrders_OrderCancelled()
    //{
    //    // Arrange 
    //    var errorCode = (byte)0x5;

    //    _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

    //    PrepareTest(false);

    //    // Create parameter set and then the order
    //    var factory = new TestOrderFactory(_towerRequestAnswerDelegate, _dateTimeService, _benchLogger);

    //    var ps = new EmptyParameterSet();

    //    var order = factory.CreateOrder(ps);
    //    SetTimeoutsAndDelegate(order);

    //    // Act
    //    _processor.AddOrder(order);
    //    _processor.Runner();

    //    Thread.Sleep(10);

    //    _processor.CancelRunningOrders(errorCode);

    //    // Assert
    //    Assert.That(!_processor.IsRunnerStopped);
    //    Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //    Assert.That(!order.WasSuccessful);
    //    Assert.That(order.IsCancelled);

    //    TestTearDown();
    //}

    [Test]
    public void CancelAllOrders_SingleNewOrder_OrdersAreCancelled()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        _processor.AddOrder(order);

        // Act
        _processor.CancelAllOrders();
        _processor.Runner();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!_processor.IsRunnerStopped);
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
            Assert.That(!order.WasSuccessful);
            Assert.That(order.IsCancelled);
            Assert.That(_isBusinesDelegateFired, Is.False);
            Assert.That(_processor.IsAnyOrderToProcess, Is.EqualTo(false));
        }

        TestTearDown();
    }

    //[Test]
    //public void CancelRunningOrders_SingleNewOrderHardwareError_OrdersAreCancelled()
    //{
    //    // Arrange 
    //    byte errorCode = 199;

    //    _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

    //    PrepareTest(false);

    //    // Create parameter set and then the order
    //    var factory = new TestOrderFactory(_towerRequestAnswerDelegate, _dateTimeService, _benchLogger);

    //    var ps = new EmptyParameterSet();
    //    var order = factory.CreateOrder(ps);


    //    SetTimeoutsAndDelegate(order);

    //    order.RequestSpecs[0].RequestAnswerSteps[0].Timeout = 2000;

    //    _processor.AddOrder(order);
    //    _processor.Runner();

    //    // Act
    //    _processor.CancelRunningOrders(errorCode);


    //    // Assert
    //    Assert.That(!_processor.IsRunnerStopped);
    //    Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //    Assert.That(!order.WasSuccessful);
    //    Assert.That(order.IsCancelled);
    //    Assert.That(_processor.IsAnyOrderToProcess, Is.EqualTo(false));

    //    TestTearDown();
    //}

    [Test]
    public void Runner_NoOrder_DoesNotThrow()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest();

        // Act
        Assert.DoesNotThrow(() =>
        {
            _processor.Runner();
        });

        // Assert
        Assert.That(!_processor.IsRunnerStopped);

        TestTearDown();
    }

    //[Test]
    //public void CancelRunningOrders_SingleNewOrderBusinessLogicError_OrdersAreCancelled()
    //{
    //    // Arrange 
    //    byte errorCode = 98;

    //    _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

    //    PrepareTest(false);

    //    // Create parameter set and then the order
    //    var factory = new TestOrderFactory(_towerRequestAnswerDelegate, _dateTimeService, _benchLogger);

    //    var ps = new EmptyParameterSet();
    //    var order = factory.CreateOrder(ps);
    //    SetTimeoutsAndDelegate(order);
    //    order.RequestSpecs[0].RequestAnswerSteps[0].Timeout = 2000;

    //    _processor.AddOrder(order);
    //    _processor.Runner();

    //    // Act
    //    _processor.CancelRunningOrders(errorCode);


    //    // Assert
    //    Assert.That(!_processor.IsRunnerStopped);
    //    Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //    Assert.That(!order.WasSuccessful);
    //    Assert.That(order.IsCancelled);

    //    //Assert.That(_towerRequestAnswerDelegate.MethodNameFired.Any(), Is.False);

    //    Assert.That(_processor.IsAnyOrderToProcess, Is.EqualTo(false));

    //    TestTearDown();
    //}

    [Test]
    public void CancelAllOrders_3NewOrders_OrdersAreCancelled()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        var order2 = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order2);

        var order3 = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order3);

        _processor.AddOrder(order);
        _processor.AddOrder(order2);
        _processor.AddOrder(order3);

        // Act
        _processor.CancelAllOrders();
        _processor.Runner();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!_processor.IsRunnerStopped);
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
            Assert.That(!order.WasSuccessful);
            Assert.That(order.IsCancelled);

            Assert.That(order2.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
            Assert.That(!order2.WasSuccessful);
            Assert.That(order2.IsCancelled);

            Assert.That(order3.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
            Assert.That(!order3.WasSuccessful);
            Assert.That(order3.IsCancelled);

            Assert.That(_isBusinesDelegateFired, Is.False);

            Assert.That(_processor.IsAnyOrderToProcess, Is.EqualTo(false));
        }

        TestTearDown();
    }

    [Test]
    public void CancelAllOrders_MultipleNewOrders_OrdersAreCancelled()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest(false);

        // Create parameter set and then the order
        for (var i = 0; i < 20; i++)
        {
            var order = CreateSdcpOrder();
            SetTimeoutsAndDelegate(order);

            _processor.AddOrder(order);
        }

        // Act
        _processor.CancelAllOrders();
        _processor.Runner();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(!_processor.IsRunnerStopped);
            Assert.That(_isBusinesDelegateFired, Is.False);

            Assert.That(_processor.IsAnyOrderToProcess, Is.False);
        }

        TestTearDown();
    }


    [Test]
    public void Runner_NewOrderWithCancelOrderBySourceUid_OrderCancelled()
    {
        // Arrange 
        var sourceUid = Guid.NewGuid();

        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest(false);

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        order.SourceUid = sourceUid;
        SetTimeoutsAndDelegate(order);

        // Act
        _processor.AddOrder(order);
        _processor.Runner();

        Thread.Sleep(10);

        _processor.CancelOrderBySourceUid(sourceUid);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_processor.IsRunnerStopped, Is.False);
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
            Assert.That(order.WasSuccessful, Is.False);
            Assert.That(order.IsCancelled, Is.True);
        }

        TestTearDown();
    }

    [Test]
    public void Runner_NewOrderWithReceivedMessageWrongMessage_OrderUnsuccessful()
    {

        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest();

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        // Arrange: create received messages
        var receivedMessage = new TncpInboundDataMessage();
        ReceiveMessage(receivedMessage);

        // Act
        _processor.AddOrder(order);
        _processor.Runner();

        Wait.Until(() => order.ExecutionResult == OrderExecutionResultState.Timeout);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Timeout));
            Assert.That(!order.WasSuccessful);
        }

        TestTearDown();
    }

    //[Test]
    //public void Runner_NewOrderWithOneParallelOrder_Unsuccessful()
    //{
    //    // Arrange 
    //    _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
    //    _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

    //    PrepareTest(false);

    //    // Create order
    //    var order1 = CreateLongRunningTestOrder(); 
    //    var order2 = CreateTestOrder();
    //    order2.IsNextOrderStartAllowed = true;

    //    SetTimeoutsAndDelegate(order1);
    //    SetTimeoutsAndDelegate(order2);

    //    // Act
    //    _processor.AddOrder(order2);
    //    _processor.AddOrder(order1);

    //    _processor.Runner();
    //    _processor.Runner();

    //    var count = 0;

    //    Wait.Until(() => (count = _processor.OrdersInProcessing.Count) == 2, 200);

    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(count, Is.EqualTo(2));
    //        //Assert.That(order1.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //        //Assert.That(order2.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //    }

    //    TestTearDown();
    //}

    [Test]
    public void Runner_NewOrderWithOneParallelOrder_Successful()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

        PrepareTest(false);

        // Create parameter set and then the order
        var order1 = CreateLongRunningTestOrder();
        var order2 = CreateTestOrder();
        order2.IsNextOrderStartAllowed = true;

        SetTimeoutsAndDelegate(order1);
        SetTimeoutsAndDelegate(order2);

        // Act
        _processor.AddOrder(order2);
        _processor.AddOrder(order1);

        _processor.Runner();
        _processor.Runner();

        Wait.Until(() => order1.ExecutionResult.Equals(OrderExecutionResultState.Successful));
        Wait.Until(() => order2.ExecutionResult.Equals(OrderExecutionResultState.Successful));


        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order1.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
            Assert.That(order2.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
        }

        TestTearDown();
    }

    //[Test]
    //public void Runner_NewOrderWithTwoParallelOrders_Unsuccessful()
    //{
    //    // Arrange 
    //    _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
    //    _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);
    //    _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

    //    PrepareTest(false);

    //    // Create parameter set and then the order
    //    var order1 = CreateExtraLongRunningTestOrder();
    //    var order2 = CreateLongRunningTestOrder();
    //    var order3 = CreateTestOrder();
    //    order2.IsNextOrderStartAllowed = true;
    //    order3.IsNextOrderStartAllowed = true;

    //    SetTimeoutsAndDelegate(order1);
    //    SetTimeoutsAndDelegate(order2);
    //    SetTimeoutsAndDelegate(order3);

    //    // Act
    //    _processor.AddOrder(order3);
    //    _processor.AddOrder(order2);
    //    _processor.AddOrder(order1);

    //    _processor.Runner();
    //    _processor.Runner();
    //    _processor.Runner();



    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(_processor.OrdersInProcessing.Count, Is.EqualTo(3));
    //        Wait.Until(() => order1.ExecutionResult.Equals(OrderExecutionResultState.Successful));
    //        Assert.That(order1.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //        Assert.That(order2.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //        Assert.That(order3.ExecutionResult, Is.SameAs(OrderExecutionResultState.Unsuccessful));
    //    }

    //    TestTearDown();
    //}

    [Test]
    public void Runner_NewOrderWithTwoParallelOrders_Successful()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);
        _commAdapter.ExpectedExecutionResult.Add(OrderExecutionResultState.Successful);

        PrepareTest(false);

        // Create parameter set and then the order
        var order1 = CreateExtraLongRunningTestOrder();
        var order2 = CreateLongRunningTestOrder();
        var order3 = CreateTestOrder();
        order2.IsNextOrderStartAllowed = true;
        order3.IsNextOrderStartAllowed = true;

        SetTimeoutsAndDelegate(order1);
        SetTimeoutsAndDelegate(order2);
        SetTimeoutsAndDelegate(order3);

        // Act
        _processor.AddOrder(order1);
        _processor.AddOrder(order2);
        _processor.AddOrder(order3);

        _processor.Runner();
        _processor.Runner();
        _processor.Runner();

        Wait.Until(() => order1.ExecutionResult.Equals(OrderExecutionResultState.Successful));
        Wait.Until(() => order2.ExecutionResult.Equals(OrderExecutionResultState.Successful));
        Wait.Until(() => order3.ExecutionResult.Equals(OrderExecutionResultState.Successful));

        // Assert
        using (Assert.EnterMultipleScope())
        {
            //Assert.That(_processor.OrdersInProcessing.Count, Is.EqualTo(3));
            Assert.That(order1.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
            Assert.That(order2.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
            Assert.That(order3.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
        }

        TestTearDown();
    }


    [Test]
    public void TryToExecuteOrderSync_OrderSyncWithNoReceivedMessage_OrderTimeout()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest();

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        // Act
        var result = _processor.TryToExecuteOrderSync(order);
        Wait.Until(() => _processor.OrdersInProcessing.Count == 0, 3000);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(order.ExecutionResult));
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Timeout));
        }

        TestTearDown();
    }

    [Test]
    public void TryToExecuteOrderSync_NewOrderWithReceivedMessage_OrderSuccessful()
    {
        // Arrange 
        _commAdapter = TestDataHelper.FakeIpCommunicationAdapter;

        PrepareTest();

        // Create parameter set and then the order
        var order = CreateSdcpOrder();
        SetTimeoutsAndDelegate(order);

        // Arrange: create received messages
        var receivedMessage = new SdcpInboundDataMessage();
        ReceiveMessage(receivedMessage);

        // Act
        var result = _processor.TryToExecuteOrderSync(order);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(order.ExecutionResult));
            Assert.That(order.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));

            Assert.That(order.WasSuccessful);
            Assert.That(_isBusinesDelegateFired);
        }

        TestTearDown();
    }
}