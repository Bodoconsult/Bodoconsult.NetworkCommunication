// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Configurations;

[TestFixture]
internal class NoAnswerBtcpOrderConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new NoAnswerBtcpOrderConfiguration();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.ConfigurationName, Is.EqualTo($"{BuiltinOrders.NoAnswerBtcpOrder}Configuration"));
            Assert.That(config.OrderTypeName, Is.EqualTo(BuiltinOrders.NoAnswerBtcpOrder));
            Assert.That(config.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.NoAnswerBtcpOrder}Builder"));
        }
    }

    [Test]
    public void Clone_ValidSetup_ConfigCloned()
    {
        // Arrange 
        var config = new NoAnswerBtcpOrderConfiguration();

        // Act  
        var clone = (NoAnswerBtcpOrderConfiguration)config.Clone();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone.ConfigurationName, Is.EqualTo($"{BuiltinOrders.NoAnswerBtcpOrder}Configuration"));
            Assert.That(clone.OrderTypeName, Is.EqualTo(BuiltinOrders.NoAnswerBtcpOrder));
            Assert.That(clone.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.NoAnswerBtcpOrder}Builder"));
            Assert.That(clone, Is.Not.EqualTo(config));
            Assert.That(clone.ParameterSet, Is.Null);
        }
    }
}