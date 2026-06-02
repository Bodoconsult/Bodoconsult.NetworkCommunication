// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class BtcpOrderFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new BtcpOrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            var data = factory.CurrentConfigurations;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count, Is.EqualTo(4));

            Assert.That(factory.GetConfiguration($"{BuiltinOrders.BtcpOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoAnswerBtcpOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerBtcpOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoHandshakeNoAnswerBtcpOrder}Configuration"),
                Is.Not.Null);
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var factory = new BtcpOrderFactory(TestDataHelper.DefaultOrderIdGenerator);
        var config = factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerBtcpOrder}Configuration");

        Assert.That(config, Is.Not.Null);

        // Act  
        var order = factory.CreateOrder(config, new BtcpParameterSet());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.RequestSpecs.Count, Is.Not.Zero);
        }
    }
}