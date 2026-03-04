// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.OrderBuilders;

[TestFixture]
internal class SdcpOrderBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new SdcpOrderBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.OrderTypeName, Is.EqualTo(BuiltinOrders.SdcpOrder));
            Assert.That(builder.ParameterSetType, Is.EqualTo(typeof(SdcpParameterSet)));
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var ps = new SdcpParameterSet();
        var builder = new SdcpOrderBuilder();

        // Act  
        var order = builder.CreateOrder(1, ps);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.ParameterSet.CurrentOrder, Is.EqualTo(order));

            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));

            var rs = order.RequestSpecs.First();
            Assert.That(rs.RequestAnswerSteps.Count, Is.EqualTo(1));

            var ras = rs.RequestAnswerSteps.First();
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new SdcpParameterSet();
        var builder = new SdcpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.EqualTo(0));

        // Act  
        using (Assert.EnterMultipleScope())
        {
            const string name = "RequestSpec";
            var requestSpec = builder.CreateDeviceRequestSpec(order, name);

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
        var ps = new SdcpParameterSet();
        var builder = new SdcpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.EqualTo(0));

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        // Act  
        const string rasName = "AnswerStep1";
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
        var ps = new SdcpParameterSet();
        var builder = new SdcpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.EqualTo(0));

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        var rasName = "AnswerStep1";
        var ras = builder.CreateDeviceRequestAnswerStep(requestSpec, rasName);

        Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(0));

        // Act  
        var raName = "Test";
        var ra = builder.CreateRequestAnswer(ras, raName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ra.Name, Is.EqualTo(raName));
            Assert.That(ra.CheckReceivedMessageDelegate, Is.Null);
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }
}