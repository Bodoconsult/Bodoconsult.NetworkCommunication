// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Configurations;

[TestFixture]
internal class OnlyAnswerBtcpOrderConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new OnlyAnswerBtcpOrderConfiguration();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.ConfigurationName, Is.EqualTo($"{BuiltinOrders.OnlyAnswerBtcpOrder}Configuration"));
            Assert.That(config.OrderTypeName, Is.EqualTo(BuiltinOrders.OnlyAnswerBtcpOrder));
            Assert.That(config.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.OnlyAnswerBtcpOrder}Builder"));
        }
    }

    [Test]
    public void Clone_ValidSetup_ConfigCloned()
    {
        // Arrange 
        var config = new OnlyAnswerBtcpOrderConfiguration();

        // Act  
        var clone = (OnlyAnswerBtcpOrderConfiguration)config.Clone();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone.ConfigurationName, Is.EqualTo($"{BuiltinOrders.OnlyAnswerBtcpOrder}Configuration"));
            Assert.That(clone.OrderTypeName, Is.EqualTo(BuiltinOrders.OnlyAnswerBtcpOrder));
            Assert.That(clone.OrderBuilder.GetType().Name, Is.EqualTo($"{BuiltinOrders.OnlyAnswerBtcpOrder}Builder"));
            Assert.That(clone, Is.Not.EqualTo(config));
            Assert.That(clone.ParameterSet, Is.Null);
        }
    }
}