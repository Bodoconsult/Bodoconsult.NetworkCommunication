//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using Bodoconsult.App.Benchmarking;
//using Bodoconsult.NetworkCommunication.EnumAndStates;
//using Bodoconsult.NetworkCommunication.Interfaces;
//using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
//using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
//using Bodoconsult.NetworkCommunication.Tests.Helpers;

//namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

//[TestFixture]
//internal class RequestAnswerStepTests
//{
//    private readonly TestOrderFactory _factory;
//    private readonly ITowerRequestAnswerDelegate _towerRequestAnswerDelegate;
//    private IOrder order;
//    private readonly AppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

//    public RequestAnswerStepTests()
//    {
//        var dateTimeService = new FakeDateTimeService();
//        _towerRequestAnswerDelegate = TestDataHelper.GetTowerRequestAnswerDelegate();
//        _factory = new TestOrderFactory(_towerRequestAnswerDelegate, dateTimeService, _benchLogger);
//    }

//    private IRequestSpec GetRequestSpec()
//    {
//        var ps = new EmptyParameterSet();
//        order = _factory.CreateOrder(ps);

//        var result = ps.IsValid;
//        Assert.That(!result.Any());

//        IRequestSpec request = new UpdateModeRequestSpec(ps, _towerRequestAnswerDelegate);

//        return request;
//    }

//    [Test]
//    public void Ctor_ValidRequestSpec_AllPropsSet()
//    {
//        // Arrange 
//        var requestSpec = GetRequestSpec();

//        // Act  
//        var requestStep = new RequestAnswerStep(requestSpec);

//        // Assert
//        Assert.That(requestStep.AllowedRequestAnswers, Is.Not.Null);
//        Assert.AreEqual(false, requestStep.WasSuccessful);
//    }


//    [Test]
//    public void CheckReceivedMessage_ValidMessage_Success()
//    {
//        // Arrange 
//        var sentMessage = TowerTestHelper.CreateSmdTowerUpdateModeMessage();

//        var answer = new UpdateModeRequestAnswer(_towerRequestAnswerDelegate);

//        var requestSpec = GetRequestSpec();
//        var requestStep = new RequestAnswerStep(requestSpec);
//        requestStep.AllowedRequestAnswers.Add(answer);

//        var error = new List<string>();
//        var receivedMessage = new SmdTowerUpdateModeMessage(MessageTypeEnum.Received);

//        // Act  
//        var result = requestStep.CheckReceivedMessage(sentMessage, receivedMessage, error);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(!error.Any());
//        Assert.That(answer.WasReceived);
//        Assert.That(answer.ReceivedMessage, Is.Not.Null);
//    }

//    //[Test]
//    //public void CheckReceivedMessageOverload_ValidMessage_Success()
//    //{
//    //    // Arrange 
//    //    var sentMessage = TowerTestHelper.CreateSmdTowerUpdateModeMessage();

//    //    var towerRequestAnswerDelegate = TestDataHelper.GetTowerRequestAnswerDelegate();

//    //    var answer = new UpdateModeRequestAnswer(towerRequestAnswerDelegate);

//    //    var processor = new FakeRequestStepProcessor
//    //    {
//    //        SentMessage = sentMessage
//    //    };

//    //    var requestSpec = GetRequestSpec();
//    //    var requestStep = new RequestAnswerStep(requestSpec);
//    //    requestStep.AllowedRequestAnswers.Add(answer);
//    //    requestStep.CurrentRequestStepProcessor = processor;

//    //    var receivedMessage = new SmdTowerUpdateModeMessage(MessageTypeEnum.Received);

//    //    // Act  
//    //    var result = requestStep.CheckReceivedMessage(receivedMessage);

//    //    // Assert
//    //    Assert.That(result, Is.Not.Null);
//    //    Assert.That(!result.Any());
//    //    Assert.That(answer.WasReceived);
//    //    Assert.That(answer.ReceivedMessage, Is.Not.Null);
//    //}


//    [Test]
//    public void CheckReceivedMessage_InvalidMessage_Fails()
//    {
//        // Arrange 
//        var sentMessage = TowerTestHelper.CreateSmdTowerUpdateModeMessage();

//        var answer = new UpdateModeRequestAnswer(_towerRequestAnswerDelegate);

//        var requestSpec = GetRequestSpec();
//        var requestStep = new RequestAnswerStep(requestSpec);
//        requestStep.AllowedRequestAnswers.Add(answer);

//        var error = new List<string>();
//        var receivedMessage = new SmdTowerUpdateModeMessage(MessageTypeEnum.Sent);

//        // Act  
//        var result = requestStep.CheckReceivedMessage(sentMessage, receivedMessage, error);

//        // Assert
//        Assert.That(!result);
//        Assert.That(error.Any());
//        Assert.That(!answer.WasReceived);
//        Assert.That(answer.ReceivedMessage, Is.Null);
//    }

//    [Test]
//    public void CheckReceivedMessage_ValidMessageMulitpleAnswers_Success()
//    {
//        // Arrange 
//        var ps = new DefaultParameterSet { TowerSn = 1234 };

//        var sentMessage = TowerTestHelper.CreateSmdTowerDataMessage(SmdTowerCommands.IdTowerCommandState, ps);

//        var requestSpec = GetRequestSpec();
//        var requestStep = new RequestAnswerStep(requestSpec);

//        IRequestAnswer answer = new RegularStateRequestBAnswer(_towerRequestAnswerDelegate);
//        requestStep.AllowedRequestAnswers.Add(answer);

//        answer = new RegularStateRequestSAnswer(_towerRequestAnswerDelegate);
//        requestStep.AllowedRequestAnswers.Add(answer);

//        var error = new List<string>();
//        var receivedMessage = new SmdTowerDataMessage(ps.TowerSn.ToString(), sentMessage.BlockAndRc, SmdTowerCommands.IdTowerCommandB, MessageTypeEnum.Received);

//        // Act  
//        var result = requestStep.CheckReceivedMessage(sentMessage, receivedMessage, error);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(!error.Any());
//    }

//    [Test]
//    public void HandleResult_ValidMessage_Success()
//    {
//        // Arrange 
//        var sentMessage = TowerTestHelper.CreateSmdTowerUpdateModeMessage();

//        var answer = new UpdateModeRequestAnswer(_towerRequestAnswerDelegate);

//        var requestStepProcessor = new FakeRequestStepProcessor();

//        var requestSpec = GetRequestSpec();
//        requestSpec.CurrentRequestStepProcessor = requestStepProcessor;

//        var requestStep = new RequestAnswerStep(requestSpec);
//        requestStep.AllowedRequestAnswers.Add(answer);
//        requestStepProcessor.RequestSpec = requestSpec;

//        var error = new List<string>();
//        var receivedMessage = new SmdTowerUpdateModeMessage(MessageTypeEnum.Received);

//        var firstResult = requestStep.CheckReceivedMessage(sentMessage, receivedMessage, error);

//        Assert.That(firstResult);
//        Assert.That(!error.Any());
//        Assert.That(answer.WasReceived);
//        Assert.That(answer.ReceivedMessage, Is.Not.Null);

//        // Act  
//        var result = requestStep.HandleResult();

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result.ExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));
//    }
//}