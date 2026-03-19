// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
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
        var data = factory.CurrentConfigurations;
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.EqualTo(3));

        Assert.That(factory.GetConfiguration($"{BuiltinOrders.BtcpOrder}Configuration"), Is.Not.Null);
        Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoAnswerBtcpOrder}Configuration"), Is.Not.Null);
        Assert.That(factory.GetConfiguration($"{BuiltinOrders.NoHandshakeNoAnswerBtcpOrder}Configuration"), Is.Not.Null);
    }
}