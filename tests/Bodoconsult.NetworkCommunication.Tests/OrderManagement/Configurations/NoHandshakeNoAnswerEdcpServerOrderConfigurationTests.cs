// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Configurations;

[TestFixture]
internal class NoHandshakeNoAnswerEdcpServerOrderConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new NoHandshakeNoAnswerEdcpServerOrderConfiguration();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.ConfigurationName,
                Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Configuration"));
            Assert.That(config.OrderTypeName, Is.EqualTo(BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder));
            Assert.That(config.OrderBuilder.GetType().Name,
                Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Builder"));
        }
    }

    [Test]
    public void Clone_ValidSetup_ConfigCloned()
    {
        // Arrange 
        var config = new NoHandshakeNoAnswerEdcpServerOrderConfiguration();

        // Act  
        var clone = (NoHandshakeNoAnswerEdcpServerOrderConfiguration)config.Clone();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone.ConfigurationName, Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Configuration"));
            Assert.That(clone.OrderTypeName, Is.EqualTo(BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder));
            Assert.That(clone.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpServerOrder}Builder"));
            Assert.That(clone, Is.Not.EqualTo(config));
            Assert.That(clone.ParameterSet, Is.Null);
        }
    }
}
