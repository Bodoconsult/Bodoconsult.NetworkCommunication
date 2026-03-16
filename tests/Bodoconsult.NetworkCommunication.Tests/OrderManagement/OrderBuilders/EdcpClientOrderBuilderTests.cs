// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.OrderBuilders;

[TestFixture]
internal class EdcpClientOrderBuilderTests : OrderBuilderTestsBase
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new EdcpClientOrderBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.OrderTypeName, Is.EqualTo(BuiltinOrders.EdcpClientOrder));
            Assert.That(builder.ParameterSetType, Is.EqualTo(typeof(TncpParameterSet)));
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.EdcpClientOrder, builder)
        {
            OrderId = 1,
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
		};

        // Act  
        var order = builder.CreateOrder(config);

		// Assert
		using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.ParameterSet?.CurrentOrder, Is.EqualTo(order));

            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));

            var rs = (IDeviceRequestSpec)order.RequestSpecs.First();
            Assert.That(rs.RequestAnswerSteps.Count, Is.EqualTo(1));

            var ras = rs.RequestAnswerSteps.First();
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateNoAnswerDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            const string name = "RequestSpec";
            var requestSpec = builder.CreateNoAnswerDeviceRequestSpec(order, name, HandleRequestAnswerOnSuccessDelegate);

            // Assert
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateNoHandshakeNoAnswerDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            const string name = "RequestSpec";
            var requestSpec = builder.CreateNoHandshakeNoAnswerDeviceRequestSpec(order, name, HandleRequestAnswerOnSuccessDelegate);

            // Assert
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateDeviceRequestAnswerStep_ValidSetup_RequestAnswerStepCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        // Act  
        var rasName = "AnswerStep1";
        var ras = builder.CreateDeviceRequestAnswerStep(requestSpec, rasName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ras, Is.Not.Null);
            Assert.That(ras.DeviceRequestSpec, Is.EqualTo(requestSpec));
            Assert.That(requestSpec.RequestAnswerSteps.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateRequestAnswerStep_ValidSetup_RequestAnswerStepCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new EdcpClientOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        var rasName = "AnswerStep1";
        var ras = builder.CreateDeviceRequestAnswerStep(requestSpec, rasName);

        Assert.That(ras.AllowedRequestAnswers.Count, Is.Zero);

        // Act  
        var raName = "Test";
        var ra = builder.CreateRequestAnswer(ras, raName, CheckReceivedMessageDelegate, HandleRequestAnswerOnSuccessDelegate);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ra.Name, Is.EqualTo(raName));
            Assert.That(ra.CheckReceivedMessageDelegate, Is.Not.Null);
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }
}