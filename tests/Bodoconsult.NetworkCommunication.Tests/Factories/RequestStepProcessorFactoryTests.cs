//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
//using Bodoconsult.NetworkCommunication.Tests.Helpers;

//namespace Bodoconsult.NetworkCommunication.Tests.Factories;

//[TestFixture]
//internal class RequestStepProcessorFactoryTests
//{

//    [Test]
//    public void CreateProcessor_TowerRequestSpec_RequestStepProcessor()
//    {
//        // Arrange 
//        var ps = new EmptyParameterSet();

//        var towerRequestAnswerDelegate = TestDataHelper.GetTowerRequestAnswerDelegate();

//        var request = new UpdateModeRequestSpec(ps, towerRequestAnswerDelegate);

//        var towerServer = TestDataHelper.GetTowerServer();
//        var factory = new RequestStepProcessorFactory();

//        // Act  
//        var result = factory.CreateProcessor(request, towerServer);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.AreEqual(typeof(Business.TowerOrderManagement.Processors.RequestStepProcessor), result.GetType());
//    }

//    [Test]
//    public void CreateProcessor_StSysInternalTowerRequestSpec_StSysInternalRequestStepProcessor()
//    {
//        // Arrange 
//        var ps = TestDataHelper.GetSDataBlockParameterSet();

//        var towerRequestAnswerDelegate = TestDataHelper.GetTowerRequestAnswerDelegate();

//        var request = new FindSlotProcessRequestSpec(ps, towerRequestAnswerDelegate);

//        var towerServer = TestDataHelper.GetTowerServer();
//        var factory = new RequestStepProcessorFactory();

//        // Act  
//        var result = factory.CreateProcessor(request, towerServer);

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.AreEqual(typeof(Business.TowerOrderManagement.Processors.InternalRequestStepProcessor), result.GetType());
//    }
//}