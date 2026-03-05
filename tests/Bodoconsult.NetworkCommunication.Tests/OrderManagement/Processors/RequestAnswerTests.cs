// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class RequestAnswerTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var type = typeof(BasicInboundDatablock);
        const string name = "Test";

        // Act  
        var ra = new RequestAnswer(true, type, name, CheckReceivedMessageDelegate);

        // Assert
        Assert.That(ra.HasDatablock, Is.True);
        Assert.That(ra.DataBlockType, Is.EqualTo(type));
        Assert.That(ra.Name, Is.EqualTo(name));

        Assert.That(ra.WasReceived, Is.False);
        Assert.That(ra.ReceivedMessage, Is.Null);
        Assert.That(ra.HandleRequestAnswerOnSuccessDelegate, Is.Null);
        Assert.That(ra.CheckReceivedMessageDelegate, Is.Null);
    }

    [Test]
    public void SetWasReceived_ValidMessage_PropsSetCorrectly()
    {
        // Arrange 
        var type = typeof(BasicInboundDatablock);
        const string name = "Test";

        var ra = new RequestAnswer(true, type, name, CheckReceivedMessageDelegate);

        var msg = new SdcpInboundDataMessage();

        // Act  
        ra.SetWasReceived(msg);

        // Assert
        Assert.That(ra.WasReceived, Is.True);
        Assert.That(ra.ReceivedMessage, Is.EqualTo(msg));
    }


    [Test]
    public void CheckReceivedMessageDelegate_ValidMessages_PropsSetCorrectly()
    {
        // Arrange 
        var type = typeof(BasicInboundDatablock);
        const string name = "Test";

        var ra = new RequestAnswer(true, type, name, CheckReceivedMessageDelegate);

        var outboundMsg = new SdcpOutboundDataMessage();
        var msg = new SdcpInboundDataMessage();

        var errors = new List<string>();

        // Act  
        ra.CheckReceivedMessageDelegate.Invoke(ra, outboundMsg, msg, errors);

        // Assert
        Assert.That(ra.WasReceived, Is.True);
        Assert.That(ra.ReceivedMessage, Is.EqualTo(msg));
    }

    [Test]
    public void Dispose_ValidMessages_PropsSetCorrectly()
    {
        // Arrange 
        var type = typeof(BasicInboundDatablock);
        const string name = "Test";

        var ra = new RequestAnswer(true, type, name, CheckReceivedMessageDelegate);
        ra.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;

        // Act  
        ra.Dispose();

        // Assert
        Assert.That(ra.WasReceived, Is.False);
        Assert.That(ra.ReceivedMessage, Is.Null);
        Assert.That(ra.CheckReceivedMessageDelegate, Is.Null);
        Assert.That(ra.HandleRequestAnswerOnSuccessDelegate, Is.Null);
    }

    [Test]
    public void Reset_ValidMessages_PropsSetCorrectly()
    {
        // Arrange 
        var type = typeof(BasicInboundDatablock);
        const string name = "Test";

        var ra = new RequestAnswer(true, type, name, CheckReceivedMessageDelegate);
        ra.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;

        // Act  
        ra.Reset();

        // Assert
        Assert.That(ra.WasReceived, Is.False);
        Assert.That(ra.ReceivedMessage, Is.Null);
        Assert.That(ra.CheckReceivedMessageDelegate, Is.Null);
        Assert.That(ra.HandleRequestAnswerOnSuccessDelegate, Is.Null);
    }

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage message, object transportObject, IParameterSet parameterSet)
    {
        // Do nothing
        return new MessageHandlingResult
            {
                Error = 0,
                ExecutionResult = OrderExecutionResultState.Successful
            };
    }

    private bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage receivedMessage, IList<string> errors)
    {
        requestAnswer.SetWasReceived(receivedMessage);
        return true;
    }
}