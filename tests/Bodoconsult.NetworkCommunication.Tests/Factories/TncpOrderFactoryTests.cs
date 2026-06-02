// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class TncpOrderFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new TncpOrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            var data = factory.CurrentConfigurations;
            Assert.That(data, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(data);
            Assert.That(data.Count, Is.EqualTo(4));

            Assert.That(factory.GetConfiguration($"{BuiltinOrders.TncpOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoAnswerTncpOrder}Configuration"), Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoHandshakeNoAnswerTncpOrder}Configuration"),
                Is.Not.Null);
            Assert.That(factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerTncpOrder}Configuration"), Is.Not.Null);
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var factory = new TncpOrderFactory(TestDataHelper.DefaultOrderIdGenerator);
        var config = factory.GetConfiguration($"{BuiltinOrders.OnlyAnswerTncpOrder}Configuration");

        Assert.That(config, Is.Not.Null);

        // Act  
        var order = factory.CreateOrder(config, new TncpParameterSet());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.RequestSpecs.Count, Is.Not.Zero);
        }
    }
}