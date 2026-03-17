// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Configurations;

[TestFixture]
internal class NoAnswerEdcpServerOrderConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new NoAnswerEdcpServerOrderConfiguration();

        // Assert
        Assert.That(config.ConfigurationName, Is.EqualTo($"{BuiltinOrders.NoAnswerEdcpServerOrder}Configuration"));
        Assert.That(config.OrderTypeName, Is.EqualTo(BuiltinOrders.NoAnswerEdcpServerOrder));
        Assert.That(config.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.NoAnswerEdcpServerOrder}Builder"));
    }
}