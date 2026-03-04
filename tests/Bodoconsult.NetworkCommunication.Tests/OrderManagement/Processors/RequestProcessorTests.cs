// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
        var device = TestDataHelper.CreateDevice();

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        // Act  
        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Assert
        Assert.That(rp.Order, Is.EqualTo(order));
        Assert.That(rp.CurrentRequestStepProcessor, Is.Null);
        Assert.That(rp.CurrentTask, Is.Null);
        Assert.That(rp.IsCancelled, Is.False);
    }

    [Test]
    public void ExecuteOrder_ValidSetupNoMessageReceived_Timeout()
    {
        // Arrange 
        var device = TestDataHelper.CreateDevice();
        device.LoadCommAdapter(TestDataHelper.FakeOrderManagementCommunicationAdapter);

        var ps = new SdcpParameterSet();

        var order = TestDataHelper.CreateSdcpOrder(ps);

        IRequestStepProcessorFactory requestStepProcessorFactory = new RequestStepProcessorFactory();

        var rp = new RequestProcessor(order, requestStepProcessorFactory, device);

        // Act  
        var result = rp.ExecuteOrder();

        // Assert
        Assert.That(result, Is.EqualTo(OrderExecutionResultState.Timeout));
    }

}