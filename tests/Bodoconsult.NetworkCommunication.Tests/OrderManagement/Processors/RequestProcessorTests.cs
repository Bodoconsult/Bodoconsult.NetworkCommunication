// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class RequestProcessorTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        // Act  
        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(rp.Order, Is.EqualTo(order));
            Assert.That(rp.CurrentRequestStepProcessor, Is.Null);
            Assert.That(rp.CurrentTask, Is.Null);
            Assert.That(rp.IsCancelled, Is.False);
        }
    }

    [Test]
    public void ExecuteOrder_NoMessageReceived_Timeout()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Act  
        var result = rp.ExecuteOrder();

        // Assert
        Assert.That(result, Is.EqualTo(OrderExecutionResultState.Timeout));
    }

    [Test]
    public void ExecuteOrder_NormalOrderMessageReceived_Successful()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        IOrderExecutionResultState result = OrderExecutionResultState.NotProcessed;

        var task = Task.Run(() =>
        {
            result = rp.ExecuteOrder();

        });

        // Act  
        Wait.Until(() => rp.CurrentRequestStepProcessor != null);

        IInboundDataMessage message = new SdcpInboundDataMessage();
        rp.CheckReceivedMessage(message);

        task.Wait(5000);

        // Assert
        Assert.That(result, Is.EqualTo(OrderExecutionResultState.Successful));
    }

    [Test]
    public void ExecuteOrder_NoAnswerOrder_Successful()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateNoAnswerSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Act  
        var result = rp.ExecuteOrder();

        // Assert
        Assert.That(result, Is.EqualTo(OrderExecutionResultState.Successful));
    }

    [Test]
    public void ExecuteOrder_NoHandshakeNoAnswerOrder_Successful()
    {
        // Arrange 
        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateNoHandshakeNoAnswerSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Act  
        var result = rp.ExecuteOrder();

        // Assert
        Assert.That(result, Is.EqualTo(OrderExecutionResultState.Successful));
    }
}