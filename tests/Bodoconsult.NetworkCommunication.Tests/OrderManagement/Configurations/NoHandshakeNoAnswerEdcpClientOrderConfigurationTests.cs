// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Configurations;

[TestFixture]
internal class NoHandshakeNoAnswerEdcpClientOrderConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new NoHandshakeNoAnswerEdcpClientOrderConfiguration();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.ConfigurationName,
                Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder}Configuration"));
            Assert.That(config.OrderTypeName, Is.EqualTo(BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder));
            Assert.That(config.OrderBuilder.GetType().Name,
                Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder}Builder"));
        }
    }

    [Test]
    public void Clone_ValidSetup_ConfigCloned()
    {
        // Arrange 
        var config = new NoHandshakeNoAnswerEdcpClientOrderConfiguration();

        // Act  
        var clone = (NoHandshakeNoAnswerEdcpClientOrderConfiguration)config.Clone();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone.ConfigurationName, Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder}Configuration"));
            Assert.That(clone.OrderTypeName, Is.EqualTo(BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder));
            Assert.That(clone.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.NoHandshakeNoAnswerEdcpClientOrder}Builder"));
            Assert.That(clone, Is.Not.EqualTo(config));
            Assert.That(clone.ParameterSet, Is.Null);
        }
    }
}
