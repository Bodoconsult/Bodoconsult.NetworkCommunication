// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
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
        var data = factory.CurrentConfigurations;
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.EqualTo(3));

        Assert.That(factory.GetConfiguration($"{BuiltinOrders.TncpOrder}Configuration"), Is.Not.Null);
        Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoAnswerTncpOrder}Configuration"), Is.Not.Null);
        Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoHandshakeNoAnswerTncpOrder}Configuration"), Is.Not.Null);
    }
}