// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class OrderReceiverTests
{
    private readonly IList<IInboundDataMessage> _receivedMessage = new List<IInboundDataMessage>();
    private readonly IAppLoggerProxy _logger = TestDataHelper.GetFakeAppLoggerProxy();

    private bool _wasFired;

    private IRequestProcessor _processor;

    [OneTimeTearDown]
    public void Cleanup()
    {
        _logger.Dispose();
        _processor.Dispose();
    }

    /// <summary>
    /// Set shorter timeouts for all process steps to avoid hanging test runs
    /// </summary>
    /// <param name="order"></param>
    /// <param name="timeOut"></param>
    private void SetTimeoutsAndDelegate(IOrder order, int timeOut = 1123)
    {
        order.IsRunningSync = true;

        foreach (var spec in order.RequestSpecs)
        {

            spec.RequestAnswerStepIsStartedDelegate = SendReceivedMessage;

            foreach (var step in spec.RequestAnswerSteps)
            {
                if (step.Timeout < timeOut)
                {
                    step.Timeout = timeOut;
                }

                if (step.Timeout > timeOut * 2)
                {
                    step.Timeout = timeOut * 2;
                }
            }

        }
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

        Debug.Print($"TOPTests: Receiving message with command {msg.ToInfoString()} started");
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
    /// Simulates <see cref="IRequestProcessor.CheckReceivedMessage"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool OrderReceiverCheckMessageDelegate(IInboundDataMessage message)
    {
        _wasFired = true;
        return true;
    }

    [Test]
    public void AddReceivedMessage_ValidMessage_DelegateWasFired()
    {
        // Arrange 
        var r = new OrderReceiver(_logger)
        {
            IsReceivedMessageProcessingActivated = true,
            OrderReceiverCheckMessageDelegate = OrderReceiverCheckMessageDelegate
        };

        var receivedMessage = new RawInboundDataMessage();

        // Act  
        r.AddReceivedMessage(receivedMessage);

        Wait.Until(() => _wasFired, 5000);

        // Assert
        Assert.That(_wasFired);

    }




    //[Test]
    //public void ExecuteOrder_WithReceivedMessage_Success()
    //{
    //    // Arrange 

    //    var towerRequestAnswerDelegate = TestDataHelper.GetTowerRequestAnswerDelegate();

    //    var dateTimeSevice = new FakeDateTimeService();

    //    var benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    //    var factory = new TestOrderFactory(towerRequestAnswerDelegate, dateTimeSevice, benchLogger);

    //    var ps = new EmptyParameterSet();

    //    var order = factory.CreateOrder(ps);
    //    SetTimeoutsAndDelegate(order);

    //    var stepFactory = new RequestStepProcessorFactory();

    //    var ts = TestDataHelper.GetTowerServer();

    //    _processor = new RequestProcessor(order, stepFactory, ts);
    //    _processor.PrepareTheChain();

    //    var receiver = new OrderReceiver(_logger)
    //    {
    //        IsReceivedMessageProcessingActivated = true,
    //        OrderReceiverCheckMessageDelegate = receivedMessage1 => _processor.CheckReceivedMessage((IInboundDataMessage)receivedMessage1)
    //    };

    //    var result = (IOrderExecutionResultState)OrderExecutionResultState.Error;

    //    var receivedMessage = new SmdTowerUpdateModeMessage(MessageTypeEnum.Received);

    //    // Act
    //    var task = Task.Run(() =>
    //    {
    //        result = _processor.ExecuteOrder();
    //    });

    //    Thread.Sleep(10);
    //    receiver.AddReceivedMessage(receivedMessage);

    //    task.Wait();

    //    // Assert
    //    Assert.That(result, Is.EqualTo(OrderExecutionResultState.Successful));
    //    Assert.That(order.WasSuccessful, Is.True);
    //    var isFound = towerRequestAnswerDelegate.MethodNameFired.Any(x => x.Contains("HandleUpdateModeRequestAnswer"));
    //    Assert.That(isFound, Is.True);
    //}
}