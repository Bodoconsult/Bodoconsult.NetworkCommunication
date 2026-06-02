// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class EdcpServerOrderFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new EdcpServerOrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            var data = factory.CurrentConfigurations;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count, Is.EqualTo(4));

            Assert.That(factory.GetConfiguration($"{BuiltinOrders.EdcpServerOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoAnswerEdcpServerOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerEdcpServerOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Configuration"),
                Is.Not.Null);
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var factory = new EdcpServerOrderFactory(TestDataHelper.DefaultOrderIdGenerator);
        var config = factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerEdcpServerOrder}Configuration");

        Assert.That(config, Is.Not.Null);

        // Act  
        var order = factory.CreateOrder(config, new EdcpParameterSet());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.RequestSpecs.Count, Is.Not.Zero);
        }
    }
}
