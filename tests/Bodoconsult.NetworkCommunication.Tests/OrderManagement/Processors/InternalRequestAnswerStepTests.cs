// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class InternalRequestAnswerStepTests
{
    private bool _isHandleResultFired;

    [SetUp]
    public void Setup()
    {
        _isHandleResultFired = false;
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var ps = new EmptyParameterSet();
        var ir = new InternalRequestSpec("Test", ps);

        // Act  
        var irs = new InternalRequestAnswerStep(ir);

		// Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(irs.RequestSpec, Is.EqualTo(ir));
            Assert.That(irs.InternalRequestSpec, Is.EqualTo(ir));
        }
    }

    [Test]
    public void HandleResult_ValidSetup_Successful()
    {
        // Arrange 
        var ps = new EmptyParameterSet();
        var ir = new InternalRequestSpec("Test", ps);

        var answer = new RequestAnswer(false, null, "TestAnswer", CheckReceivedMessageDelegate);
        answer.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;

        var irs = new InternalRequestAnswerStep(ir);
        irs.AllowedRequestAnswers.Add(answer);

        // Act  
        var result = irs.HandleResult();

		// Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error, Is.Zero);
            Assert.That(result.ExecutionResult, Is.EqualTo(OrderExecutionResultState.Successful));
            Assert.That(_isHandleResultFired, Is.True);
        }
    }

    private bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors)
    {
        return true;
    }

    [Test]
    public void HandleResult_NoAnswer_Unsuccessful()
    {
        // Arrange 
        var ps = new EmptyParameterSet();
        var ir = new InternalRequestSpec("Test", ps);

        var irs = new InternalRequestAnswerStep(ir);

        // Act  
        var result = irs.HandleResult();

		// Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error, Is.Not.EqualTo(0));
            Assert.That(result.ExecutionResult, Is.EqualTo(OrderExecutionResultState.Unsuccessful));
        }
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