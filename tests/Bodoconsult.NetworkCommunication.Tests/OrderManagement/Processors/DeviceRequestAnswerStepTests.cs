// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class DeviceRequestAnswerStepTests
{
    private bool _isHandleResultFired;
    private bool _isCheckReceivedFired;
    private bool _isStateSetFired;
    private bool _checkResult;

    [SetUp]
    public void Setup()
    {
        _isHandleResultFired = false;
        _isCheckReceivedFired = false;
        _isStateSetFired = false;
        _checkResult = true;
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var order = TestDataHelper.CreateOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);

        var dr = new DeviceRequestSpec("Test", ps);

        // Act  
        var irs = new DeviceRequestAnswerStep(dr);

        // Assert
        Assert.That(irs.RequestSpec, Is.EqualTo(dr));
        Assert.That(irs.DeviceRequestSpec, Is.EqualTo(dr));
    }

    [Test]
    public void HandleResult_ValidSetup_Successful()
    {
        // Arrange 
        var order = TestDataHelper.CreateOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);
        var dr = new DeviceRequestSpec("Test", ps);

        var answer = new RequestAnswer(false, null, "TestAnswer");
        answer.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;

        var irs = new DeviceRequestAnswerStep(dr);
        irs.AllowedRequestAnswers.Add(answer);

        order.IsCancelled = true;

        // Act  
        var result = irs.HandleResult();

        // Assert
        Assert.That(result.Error, Is.EqualTo(2));
        Assert.That(result.ExecutionResult, Is.EqualTo(OrderExecutionResultState.Unsuccessful));
        Assert.That(_isHandleResultFired, Is.False);
    }

    [Test]
    public void HandleResult_NoAnswer_Unsuccessful()
    {
        // Arrange 
        var order = TestDataHelper.CreateOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);
        var dr = new DeviceRequestSpec("Test", ps);

        var irs = new DeviceRequestAnswerStep(dr);

        // Act  
        var result = irs.HandleResult();

        // Assert
        Assert.That(result.Error, Is.Not.EqualTo(0));
        Assert.That(result.ExecutionResult, Is.EqualTo(OrderExecutionResultState.Unsuccessful));
    }

    [Test]
    public void CheckReceivedMessage_ReceivedMessage_Successful()
    {
        // Arrange 
        var order = TestDataHelper.CreateOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);
        var dr = new DeviceRequestSpec("Test", ps);

        dr.RequestStepProcessorSetResultDelegate = RequestStepProcessorSetResultDelegate;

        var answer = new RequestAnswer(false, null, "TestAnswer");
        answer.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;
        answer.CheckReceivedMessageDelegate = CheckReceivedMessageDelegate;

        var irs = new DeviceRequestAnswerStep(dr);
        irs.AllowedRequestAnswers.Add(answer);

        var outboundMsg = new SdcpOutboundDataMessage();
        var msg = new SdcpInboundDataMessage();

        dr.CurrentSentMessage = outboundMsg;

        // Act  
        var result = irs.CheckReceivedMessage(msg).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(_isCheckReceivedFired, Is.True);
        Assert.That(_isStateSetFired, Is.True);
    }

    [Test]
    public void CheckReceivedMessage_ReceivedMessageNotFiiting_Unsuccessful()
    {
        // Arrange 
        _checkResult = false;

        var order = TestDataHelper.CreateOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);
        var dr = new DeviceRequestSpec("Test", ps);

        dr.RequestStepProcessorSetResultDelegate = RequestStepProcessorSetResultDelegate;

        var answer = new RequestAnswer(false, null, "TestAnswer");
        answer.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;
        answer.CheckReceivedMessageDelegate = CheckReceivedMessageDelegate;

        var irs = new DeviceRequestAnswerStep(dr);
        irs.AllowedRequestAnswers.Add(answer);

        var outboundMsg = new SdcpOutboundDataMessage();
        var msg = new SdcpInboundDataMessage();

        dr.CurrentSentMessage = outboundMsg;

        // Act  
        var result = irs.CheckReceivedMessage(msg).ToList();

        // Assert
        Assert.That(result.Count, Is.Not.EqualTo(0));
        Assert.That(_isCheckReceivedFired, Is.True);
        Assert.That(_isStateSetFired, Is.False);
    }

    private void RequestStepProcessorSetResultDelegate(IOrderExecutionResultState state)
    {
        _isStateSetFired = true;
        // Do nothing
    }

    private bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors)
    {
        _isCheckReceivedFired = true;
        if (_checkResult)
        {
            return true;
        }
        errors.Add("Received message not fitting");
        return false;
    }

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage message, object transportObject, IParameterSet parameterSet)
    {
        _isHandleResultFired = true;
        return new MessageHandlingResult
        {
            Error = 0,
            ExecutionResult = OrderExecutionResultState.Successful,
        };
    }


}